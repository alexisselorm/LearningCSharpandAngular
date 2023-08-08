using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.Constants;
using MyBGList.Models;
using MyBGList.Models.Csv;
using System.Globalization;

namespace MyBGList.Controllers
{
    [Route("[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Authorize(Roles = RoleNames.Administrator)]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BggRecord> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApiUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedController(ILogger<BggRecord> logger, ApplicationDbContext context, IWebHostEnvironment env, UserManager<ApiUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _env = env;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpPut(Name = "SeedGame")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> BoardGameData(int? id = null)
        {
            var config = new CsvConfiguration(CultureInfo.GetCultureInfo("pt-BR"))
            {
                HasHeaderRecord = true,
                Delimiter = ";",
            };
            using var reader = new StreamReader(System.IO.Path.Combine(_env.ContentRootPath, "Data/bgg_dataset.csv"));
            using var csv = new CsvReader(reader, config);
            var existingGames = await _context.BoardGames.ToDictionaryAsync(bg => bg.Id);
            var existingDomains = await _context.Domains.ToDictionaryAsync(d => d.Name);
            var existingMechanics = await _context.Mechanics.ToDictionaryAsync(m => m.Name);
            var now = DateTime.Now;

            //EXECUTE THE SEEDER
            var records = csv.GetRecords<BggRecord>();
            var skippedRows = 0;

            foreach (var record in records)
            {
                if (!record.ID.HasValue
                    || string.IsNullOrEmpty(record.Name)
                    || existingGames.ContainsKey(record.ID.Value)
                    || (id.HasValue
                    && id.Value != record.ID.Value)
                    )
                {
                    skippedRows++;
                    continue;
                }
                var boardgame = new BoardGame()
                {
                    Id = record.ID.Value,
                    Name = record.Name,
                    BGGRank = record.BGGRank ?? 0,
                    ComplexityAverage = record.ComplexityAverage ?? 0,
                    MaxPlayers = record.MaxPlayers ?? 0,
                    MinPlayers = record.MinPlayers ?? 0,
                    MinAge = record.MinAge ?? 0,
                    OwnedUsers = record.OwnedUsers ?? 0,
                    PlayTime = record.PlayTime ?? 0,
                    RatingAverage = record.RatingAverage ?? 0,
                    UsersRated = record.UsersRated ?? 0,
                    Year = record.YearPublished ?? 0,
                    CreatedDate = now,
                    LastModifiedDate = now,
                };
                _context.BoardGames.Add(boardgame);

                if (!string.IsNullOrEmpty(record.Domains))
                    foreach (var domainName in record.Domains
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Distinct(StringComparer.InvariantCultureIgnoreCase))
                    {
                        var domain = existingDomains.GetValueOrDefault(domainName);
                        if (domain == null)
                        {
                            domain = new Domain()
                            {
                                Name = domainName,
                                CreatedDate = now,
                                LastModifiedDate = now,
                            };
                            _context.Domains.Add(domain);
                            existingDomains.Add(domainName, domain);
                        }
                        _context.BoardGames_Domains.Add(new BoardGames_Domains()
                        {
                            BoardGame = boardgame,
                            Domain = domain,
                            CreatedDate = now

                        });
                    }
                if (!string.IsNullOrEmpty(record.Mechanics))
                    foreach (var mechanicName in record.Mechanics
                        .Split(',', StringSplitOptions.TrimEntries)
                        .Distinct(StringComparer.InvariantCultureIgnoreCase))
                    {
                        var mechanic = existingMechanics.GetValueOrDefault(mechanicName);
                        if (mechanic == null)
                        {
                            mechanic = new Mechanic()
                            {
                                Name = mechanicName,
                                CreatedDate = now,
                                LastModifiedDate = now,

                            };
                            _context.Mechanics.Add(mechanic);
                            existingMechanics.Add(mechanicName, mechanic);
                        }
                        _context.BoardGames_Mechanics.Add(new BoardGames_Mechanics()
                        {
                            BoardGame = boardgame,
                            Mechanic = mechanic,
                            CreatedDate = now,
                        });

                    }
            }
            //SAVE
            using var transaction = _context.Database.BeginTransaction();
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT BoardGames ON");
            await _context.SaveChangesAsync();
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT BoardGames OFF");
            transaction.Commit();


            //RECAP
            return new JsonResult(new
            {
                BoardGames = _context.BoardGames.Count(),
                Domains = _context.Domains.Count(),
                Mechanics = _context.Mechanics.Count(),
                SkippedRows = skippedRows
            });
        }


        [HttpPost(Name = "SeedAuth")]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<IActionResult> AuthData()
        {
            int rolesCreated = 0;
            int usersAddToRoles = 0;

            //Create admin and mod roles
            if (!await _roleManager.RoleExistsAsync(RoleNames.Moderator))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole(RoleNames.Moderator)
                    );
                rolesCreated++;
            }
            if (!await _roleManager.RoleExistsAsync(RoleNames.Administrator))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole(RoleNames.Administrator));
                rolesCreated++;
            }

            //Add users to roles
            //Mod
            var moderator = await _userManager.FindByNameAsync("AlexisModerator");
            if (moderator != null && !await _userManager.IsInRoleAsync(moderator, RoleNames.Moderator))
            {
                await _userManager.AddToRoleAsync(moderator, RoleNames.Moderator);
                usersAddToRoles++;
            }
            //Admin
            var admin = await _userManager.FindByNameAsync("AlexisAdmin");
            if (admin != null && !await _userManager.IsInRoleAsync(admin, RoleNames.Administrator))
            {
                await _userManager.AddToRolesAsync(admin, new[] { RoleNames.Moderator, RoleNames.Administrator });
                usersAddToRoles++;
            }

            return new JsonResult(new
            {
                RolesCreated = rolesCreated,
                UsersAddToRoles = usersAddToRoles
            });
        }
    }
}
