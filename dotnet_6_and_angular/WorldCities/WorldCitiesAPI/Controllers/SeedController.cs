using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        //User
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public SeedController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;

        }

        [HttpGet]
        public async Task<ActionResult> Import()
        {
            if (!_env.IsDevelopment())
            {
                throw new SecurityException("Not allowed");
            }

            var path = Path.Combine(_env.ContentRootPath, "Data/Sources/worldcities.xlsx");

            using var stream = System.IO.File.OpenRead(path);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var excelPackage = new ExcelPackage(stream);

            //get the first worksheet
            var worksheet = excelPackage.Workbook.Worksheets[0];

            //define how many rows we want to process
            var nEndRow = worksheet.Dimension.End.Row;

            //initialize the record counters
            var numberOfCountriesAdded = 0;
            var numberOfCitiesAdded = 0;

            //Create a lookup dictionary containing all the countries already existing into the database (it will be empty on first run)
            var countriesByName = _context.Countries.AsNoTracking().ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

            //iterate trough all rows, skipping the first one
            for (int nRow = 2; nRow < nEndRow; nRow++)
            {
                var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];

                var countryName = row[nRow, 5].GetValue<string>();
                var iso2 = row[nRow, 6].GetValue<string>();
                var iso3 = row[nRow, 7].GetValue<string>();

                //skip this country if it already exists in the database
                if (countriesByName.ContainsKey(countryName))
                {
                    continue;
                }
                var country = new Country
                {
                    Name = countryName,
                    ISO2 = iso2,
                    ISO3 = iso3
                };

                //add the new country to the db context
                await _context.Countries.AddAsync(country);

                //store the country in our lookup dictionary 
                countriesByName.Add(countryName, country);

                //increment the counter
                numberOfCountriesAdded++;
            }
            //Save all the countries into the Database
            if (numberOfCountriesAdded > 0)
            {
                await _context.SaveChangesAsync();
            }

            // create a lookup dictionary
            // containing all the cities already existing 
            // into the Database (it will be empty on first run). 
            var cities = _context.Cities.AsNoTracking().ToDictionary(x => (
            Name: x.Name, Lat: x.Lat, Lon: x.Lon, CountryId: x.CountryId));

            //iterate through all the rows, skipping the first one
            for (int nRow = 2; nRow < nEndRow; nRow++)
            {
                var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];

                var name = row[nRow, 1].GetValue<string>();
                var nameAscii = row[nRow, 2].GetValue<string>();
                var lat = row[nRow, 3].GetValue<decimal>();
                var lon = row[nRow, 4].GetValue<decimal>();
                var countryName = row[nRow, 5].GetValue<string>();

                //retrieve country Id by countryName
                var countryId = countriesByName[countryName].Id;

                //skip this city if it already exists in the database
                if (cities.ContainsKey((Name: name, Lat: lat, Lon: lon, CountryId: countryId)))
                {
                    continue;
                }

                //Create the city entity to be added to the db
                var city = new City
                {
                    Name = name,
                    Lat = lat,
                    Lon = lon,
                    CountryId = countryId,
                };

                //add the new city to the City context
                await _context.Cities.AddAsync(city);

                //increment the counter
                numberOfCitiesAdded++;
            }
            if (numberOfCitiesAdded > 0)
            {
                await _context.SaveChangesAsync();
            }

            return new JsonResult(new
            {
                Cities = numberOfCitiesAdded,
                Countries = numberOfCountriesAdded
            });
        }


        [HttpGet]
        public async Task<ActionResult> CreateDefaultUsers()
        {
            //set the default role names
            string role_RegisteredUser = "RegisteredUser";
            string role_Administrator = "Administrator";

            //create the default roles (if they don't exist yet).
            if (await _roleManager.FindByNameAsync(role_RegisteredUser) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));
            }
            if (await _roleManager.FindByNameAsync(role_Administrator) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(role_Administrator));
            }

            //create a list to track the newly added users.
            var addedList = new List<ApplicationUser>();

            //CHECK if admin user already exists.
            var email_Admin = "admin@email.com";
            if (await _userManager.FindByNameAsync(email_Admin) == null)
            {
                //create a new admin Application user account
                var user_Admin = new ApplicationUser
                {
                    UserName = email_Admin,
                    Email = email_Admin,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };
                // insert the admin user into the DB
                await _userManager.CreateAsync(user_Admin, _configuration["DefaultPasswords:Administrator"]);

                //assign the 'RegisteredUser' and 'Administrator' roles
                await _userManager.AddToRoleAsync(user_Admin, role_RegisteredUser);
                await _userManager.AddToRoleAsync(user_Admin, role_Administrator);

                //confirm the email and remove lockout
                user_Admin.EmailConfirmed = true;
                user_Admin.LockoutEnabled = false;

                //add the admin to the added users list
                addedList.Add(user_Admin);

            }


            //CHECK if standard user already exists.
            var email_User = "user@email.com";
            if (await _userManager.FindByNameAsync(email_User) == null)
            {
                //create a new standard user Application user account
                var user_User = new ApplicationUser
                {
                    UserName = email_User,
                    Email = email_User,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };
                // insert the user into the DB
                await _userManager.CreateAsync(user_User, _configuration["DefaultPasswords:RegisteredUser"]);

                //assign the 'RegisteredUser' role
                await _userManager.AddToRoleAsync(user_User, role_RegisteredUser);

                //confirm the email and remove lockout
                user_User.EmailConfirmed = true;
                user_User.LockoutEnabled = false;

                //add the standard user to the added users list
                addedList.Add(user_User);
            }

            //if we added at least one user, persist the changes nto the DB
            if (addedList.Count > 0)
            {
                await _context.SaveChangesAsync();
            }

            return new JsonResult(new { Count = addedList.Count, Users = addedList });
        }
    }
}
