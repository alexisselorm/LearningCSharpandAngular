using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.Attributes;
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

        [HttpGet(Name = "GetDomains")]

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [ManualValidationFilter]
        public async Task<ActionResult<RestDTO<Domain[]>>> Get([FromQuery] RequestDTO<DomainDTO> input)
        {
            if (!ModelState.IsValid)
            {
                var details = new ValidationProblemDetails(ModelState);
                details.Extensions["traceId"] = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;
                if (ModelState.Keys.Any(k => k == "PageSize"))
                {
                    details.Type =
                        "https://tools.ietf.org/html/rfc7231#section-6.6.2";
                    details.Status = StatusCodes.Status501NotImplemented;
                    return new ObjectResult(details)
                    {
                        StatusCode = StatusCodes.Status501NotImplemented
                    };
                }
                else
                {
                    details.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    details.Status = StatusCodes.Status400BadRequest;
                    return new ObjectResult(details);
                }
            }
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
                RecordCount = await _context.Domains.CountAsync(),
                Links = new List<LinkDTO>{
                    new LinkDTO(Url.Action(null,"Domain",new {input.PageIndex,input.PageSize},Request.Scheme)!,"self","GET")
                }
            };
        }

        [HttpPatch(Name = "UpdateDomain")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<Domain?>> Patch(DomainDTO model)
        {
            var domain = await _context.Domains.Where(d => d.Id == model.Id).FirstOrDefaultAsync();
            var now = DateTime.Now;

            if (domain != null)
            {
                if (!string.IsNullOrEmpty(model.Name))
                    domain.Name = model.Name;

                domain.LastModifiedDate = now;

                _context.Domains.Update(domain);
                await _context.SaveChangesAsync();

            }
            return new RestDTO<Domain?>()
            {
                Data = domain,
                Links = new List<LinkDTO>{
                        new LinkDTO(Url.Action(null,"Domains",model,Request.Scheme)!,"self","PATCH"),
                    }
            };
        }

        [HttpDelete(Name = "DeleteDomain")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<Domain[]>> Delete(int[] idList)
        {
            var domains = new List<Domain>();
            foreach (var id in idList)
            {
                var domain = await _context.Domains.Where(d => d.Id == id).FirstOrDefaultAsync();

                if (domain != null)
                {
                    domains.Add(domain);
                    _context.Domains.Remove(domain);
                    await _context.SaveChangesAsync();
                }
            }
            return new RestDTO<Domain[]>
            {
                Data = domains.ToArray(),
                Links = new List<LinkDTO> {
                    new LinkDTO(Url.Action(null,"Domains",idList,Request.Scheme)!,"self","DELETE")
                }
            };
        }
    }
}