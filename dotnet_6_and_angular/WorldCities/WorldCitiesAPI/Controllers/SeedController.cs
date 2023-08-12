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

        public SeedController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
    }
}
