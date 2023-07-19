using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyBGList.Constants;
using MyBGList.DTO;
using MyBGList.Models;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace MyBGList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BoardGamesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<BoardGamesController> _logger;
        private readonly IMemoryCache _memoryCache;

        public BoardGamesController(ILogger<BoardGamesController> logger, ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _logger = logger;
            _context = context;
            _memoryCache = memoryCache;
        }

        //GET ALL BOARDGAMES
        [HttpGet(Name = "GetBoardGames")]
        [ResponseCache(CacheProfileName = "Client-120")]
        public async Task<RestDTO<BoardGame[]>> Get(
            [FromQuery] RequestDTO<BoardGameDTO> input
            )
        {
            _logger.LogInformation(CustomLogEvents.BoardGamesController_Get, "Get method started");

            var query = _context.BoardGames.AsQueryable();
            if (!string.IsNullOrEmpty(input.FilterQuery))
                query = query.Where(b => b.Name.Contains(input.FilterQuery));

            var resultCount = await query.CountAsync();
            BoardGame[]? result = null;

            var cacheKey = $"{input.GetType()}-{JsonSerializer.Serialize(input)}";
            if (!_memoryCache.TryGetValue<BoardGame[]>(cacheKey, out result))
            {
                query = query
               .OrderBy($"{input.SortColumn} {input.SortOrder}")
               .Skip(input.PageIndex * input.PageSize)
               .Take(input.PageSize);
                result = await query.ToArrayAsync();
                _memoryCache.Set(cacheKey, result, new TimeSpan(0, 2, 0));
            }

            return new RestDTO<BoardGame[]>()
            {
                Data = result,
                PageIndex = input.PageIndex,
                PageSize = input.PageSize,
                RecordCount = resultCount,
                Links = new List<LinkDTO> {
            new LinkDTO(
                Url.Action(null, "BoardGames",new {input.PageIndex,input.PageSize}, Request.Scheme)!,
                "self",
                "GET"),
        }
            };
        }

        //GET BOARDGAME BY ID
        [HttpPost("{id}")]
        [ResponseCache(CacheProfileName = "Any-60")]
        public async Task<RestDTO<BoardGame>> GetBoardGame(int id)
        {
            _logger.LogInformation("Get boardgame by id");
            BoardGame? result = null;
            var cacheKey = $"GetBoardGame-{id}";

            if (!_memoryCache.TryGetValue<BoardGame>(cacheKey, out result))
            {

                result = await _context.BoardGames.FirstOrDefaultAsync(b => b.Id == id);
                _memoryCache.Set(cacheKey, result, new TimeSpan(0, 2, 0));
            }
            return new RestDTO<BoardGame>()
            {
                Data = result,
                Links = new List<LinkDTO>
                    {
                        new LinkDTO(
                            Url.Action(null,"BoardGames",id,Request.Scheme)!,"self","GET")
                    }
            };

        }



        [HttpPatch(Name = "UpdateBoardGame")]
        [Authorize(Roles = RoleNames.Moderator)]
        [ResponseCache(CacheProfileName = "NoCache")]

        public async Task<RestDTO<BoardGame?>> Patch(BoardGameDTO model)
        {
            var boardgame = await _context.BoardGames
                .Where(b => b.Id == model.Id)
                .FirstOrDefaultAsync();

            if (boardgame != null)
            {
                if (!string.IsNullOrEmpty(model.Name))
                    boardgame.Name = model.Name;

                if (model.Year.HasValue && model.Year.Value > 0)
                    boardgame.Year = model.Year.Value;

                if (model.PlayTime.HasValue && model.PlayTime.Value > 0)
                    boardgame.PlayTime = model.PlayTime.Value;

                if (model.MinAge.HasValue && model.MinAge.Value > 0)
                    boardgame.MinAge = model.MinAge.Value;

                if (model.MaxPlayers.HasValue && model.MaxPlayers.Value > 0)
                    boardgame.MaxPlayers = model.MaxPlayers.Value;
                if (model.MinPlayers.HasValue && model.MinPlayers.Value > 0)
                    boardgame.MinPlayers = model.MinPlayers.Value;

                boardgame.LastModifiedDate = DateTime.Now;

                _context.BoardGames.Update(boardgame);
                await _context.SaveChangesAsync();


            }
            return new RestDTO<BoardGame?>()
            {
                Data = boardgame,
                Links = new List<LinkDTO>
                    {
                        new LinkDTO(
                            Url.Action(null,"BoardGames",model,Request.Scheme)!,"self","PATCH")
            }
            };
        }


        [HttpDelete(Name = "DeleteBoardGame")]
        [Authorize(Roles = RoleNames.Administrator)]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<RestDTO<BoardGame[]?>> Delete(int[] idList)
        {
            var boardgames = new List<BoardGame>();
            foreach (var id in idList)
            {
                var boardgame = await _context.BoardGames
               .Where(b => b.Id == id)
               .FirstOrDefaultAsync();

                if (boardgame != null)
                {
                    boardgames.Add(boardgame);
                    _context.BoardGames.Remove(boardgame);
                    await _context.SaveChangesAsync();
                }
            }



            return new RestDTO<BoardGame[]?>()
            {
                Data = boardgames.ToArray(),
                Links = new List<LinkDTO> {
                    new LinkDTO(
                Url.Action(null,"BoardGames",idList,Request.Scheme)!,"self","DELETE")
}
            };
        }
    }
}
