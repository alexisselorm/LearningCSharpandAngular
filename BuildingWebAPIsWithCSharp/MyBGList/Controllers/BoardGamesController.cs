using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.Constants;
using MyBGList.DTO;
using MyBGList.Models;
using System.Linq.Dynamic.Core;

namespace MyBGList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BoardGamesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<BoardGamesController> _logger;

        public BoardGamesController(ILogger<BoardGamesController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet(Name = "GetBoardGames")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<RestDTO<BoardGame[]>> Get(
            [FromQuery] RequestDTO<BoardGameDTO> input
            )
        {
            _logger.LogInformation(CustomLogEvents.BoardGamesController_Get, "Get method started");

            var query = _context.BoardGames.AsQueryable();
            if (!string.IsNullOrEmpty(input.FilterQuery))
                query = query.Where(b => b.Name.Contains(input.FilterQuery));
            query = query
                .OrderBy($"{input.SortColumn} {input.SortOrder}")
                .Skip(input.PageIndex * input.PageSize)
                .Take(input.PageSize);
            return new RestDTO<BoardGame[]>()
            {
                Data = await query.ToArrayAsync(),
                PageIndex = input.PageIndex,
                PageSize = input.PageSize,
                RecordCount = await _context.BoardGames.CountAsync(),
                Links = new List<LinkDTO> {
            new LinkDTO(
                Url.Action(null, "BoardGames",new {input.PageIndex,input.PageSize}, Request.Scheme)!,
                "self",
                "GET"),
        }
            };
        }


        [HttpPatch(Name = "UpdateBoardGame")]
        [ResponseCache(NoStore = true)]
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
        [ResponseCache(NoStore = true)]
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
