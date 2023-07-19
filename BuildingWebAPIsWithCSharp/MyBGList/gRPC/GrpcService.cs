using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MyBGList.Constants;
using MyBGList.Models;

namespace MyBGList.gRPC
{
    public class GrpcService : Grpc.GrpcBase
    {
        private readonly ApplicationDbContext _context;

        public GrpcService(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task<BoardGameResponse> GetBoardGame(BoardGameRequest request, ServerCallContext scc)
        {
            var bg = await _context.BoardGames.FirstOrDefaultAsync(bg => bg.Id == request.Id);
            var response = new BoardGameResponse();
            if (bg != null)
            {
                response.Id = bg.Id;
                response.Name = bg.Name;
                response.Year = bg.Year;
            }
            return response;
        }

        [Authorize(Roles = RoleNames.Moderator)]
        public override async Task<BoardGameResponse> UpdateBoardGame(UpdateBoardGameRequest request, ServerCallContext scc)
        {
            var boardgame = await _context.BoardGames.FirstOrDefaultAsync(bg => bg.Id == request.Id);
            var response = new BoardGameResponse();
            if (boardgame != null)
            {
                boardgame.Name = request.Name;
                _context.BoardGames.Update(boardgame);
                await _context.SaveChangesAsync();
                response.Id = boardgame.Id;
                response.Name = boardgame.Name;
                response.Year = boardgame.Year;
            }
            return response;
        }
    }
}
