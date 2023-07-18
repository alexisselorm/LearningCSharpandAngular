using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MyBGList.DTO;
using MyBGList.Extensions;
using MyBGList.Models;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace MyBGList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MechanicsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MechanicsController> _logger;
        private readonly IDistributedCache _distributedCache;

        public MechanicsController(ApplicationDbContext context, ILogger<MechanicsController> logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            _context = context;
            _distributedCache = distributedCache;
        }

        [HttpGet(Name = "GetMechanics")]
        [ResponseCache(CacheProfileName = "Any-60")]
        public async Task<RestDTO<Mechanic[]>> Get([FromQuery] RequestDTO<MechanicDTO> input)
        {
            Mechanic[]? result = null;
            var cacheKey = $"{input.GetType()} - {JsonSerializer.Serialize(input)}";
            if (!_distributedCache.TryGetValue<Mechanic[]>(cacheKey, out result))
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
                result = await query.ToArrayAsync();
                _distributedCache.Set(cacheKey, result, new TimeSpan(0, 2, 0));
            }


            return new RestDTO<Mechanic[]>()
            {
                Data = result,
                PageIndex = input.PageIndex,
                PageSize = input.PageSize,
                RecordCount = await _context.Mechanics.CountAsync(),
                Links = new List<LinkDTO>
                {
                    new LinkDTO(Url.Action(null,"Mechanics",new {input.PageIndex,input.PageSize},Request.Scheme)!,"self","GET")
                }
            };

        }

        [HttpPatch(Name = "UpdateMechanic")]
        [ResponseCache(CacheProfileName = "NoCache")]
        [Authorize]
        public async Task<RestDTO<Mechanic?>> Patch(MechanicDTO model)
        {
            var mechanic = await _context.Mechanics.Where(m => m.Id == model.Id).FirstOrDefaultAsync();
            var now = DateTime.Now;
            if (mechanic != null)
            {
                if (!string.IsNullOrEmpty(model.Name))
                {
                    mechanic.Name = model.Name;
                }
                mechanic.LastModifiedDate = now;

                _context.Mechanics.Update(mechanic);
                await _context.SaveChangesAsync();
            }
            return new RestDTO<Mechanic?>()
            {
                Data = mechanic,
                Links = new List<LinkDTO>
                    {
                        new LinkDTO(Url.Action(null,"Mechanics",model,Request.Scheme)!,"self","PATCH")
                    }
            };

        }

        [HttpDelete(Name = "DeleteMechanic")]
        [ResponseCache(CacheProfileName = "NoCache")]
        [Authorize]
        public async Task<RestDTO<Mechanic[]>> Delete(int[] idList)
        {
            var mechanics = new List<Mechanic>();
            foreach (var id in idList)
            {

                var mechanic = await _context.Mechanics.Where(m => m.Id == id).FirstOrDefaultAsync();
                if (mechanic != null)
                {
                    mechanics.Add(mechanic);
                    _context.Mechanics.Remove(mechanic);
                    await _context.SaveChangesAsync();
                }
            }
            return new RestDTO<Mechanic[]>
            {
                Data = mechanics.ToArray(),
                Links = new List<LinkDTO>
                {
             new LinkDTO(
                    Url.Action(null, "Mechanics", idList, Request.Scheme)!,"self","DELETE")
                }
            };


        }


    }
}
