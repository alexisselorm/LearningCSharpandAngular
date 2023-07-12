using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.DTO;
using MyBGList.Models;
using System.Linq.Dynamic.Core;

namespace MyBGList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MechanicsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MechanicsController> _logger;

        public MechanicsController(ApplicationDbContext context, ILogger<MechanicsController> logger)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet(Name = "GetDomains")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<RestDTO<Mechanic[]>> Get([FromQuery] RequestDTO<MechanicDTO> input)
        {
            var query = _context.Mechanics.AsQueryable();

            if (!string.IsNullOrEmpty(input.FilterQuery))
            {
                query = query.Where(m => m.Name.Contains(input.FilterQuery));
            }
            query = query.
                OrderBy($"{input.SortColumn} {input.SortOrder}").
                Skip(input.PageIndex * input.PageSize).
                Take(input.PageSize);

            return new RestDTO<Mechanic[]>()
            {
                Data = await query.ToArrayAsync(),
                PageIndex = input.PageIndex,
                PageSize = input.PageSize,
                RecordCount = await _context.Mechanics.CountAsync(),
                Links = new List<LinkDTO>
                {
                    new LinkDTO(Url.Action(null,"Mechanics",new {input.PageIndex,input.PageSize},Request.Scheme)!,"self","GET")
                }
            };

        }


    }
}
