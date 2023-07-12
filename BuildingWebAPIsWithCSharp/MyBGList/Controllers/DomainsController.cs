using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.DTO;
using MyBGList.Models;
using System.Linq.Dynamic.Core;

namespace MyBGList.Controllers
{
    [Route("[controller]")]
    public class DomainsController : ControllerBase
    {
        private readonly ILogger<DomainsController> _logger;
        private readonly ApplicationDbContext _context;

        public DomainsController(ApplicationDbContext context, ILogger<DomainsController> logger)
        {
            _logger = logger;
            _context = context;

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<RestDTO<Domain[]>> Get([FromQuery] RequestDTO<DomainDTO> input)
        {
            var query = _context.Domains.AsQueryable();

            if (!string.IsNullOrEmpty(input.FilterQuery))
            {
                query = query.Where(d => d.Name.Contains(input.FilterQuery));
            }
            query = query.
            OrderBy($"{input.SortColumn} {input.SortOrder}").
            Skip(input.PageIndex * input.PageSize).
            Take(input.PageSize);

            return new RestDTO<Domain[]>()
            {
                Data = await query.ToArrayAsync(),
                PageIndex = input.PageIndex,
                PageSize = input.PageSize,
                Links = new List<LinkDTO>{
                    new LinkDTO(Url.Action(null,"Domain",new {input.PageIndex,input.PageSize},Request.Scheme)!,"self","GET")
                }
            };



        }
    }
}