using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using MyBGList.Constants;
using MyBGList.DTO;
using MyBGList.Models;

namespace MyBGList.GraphQL
{
    public class Mutation
    {

        //BOARDGAME MUTATIONS
        [Serial]
        [Authorize(Roles = new[] { RoleNames.Moderator })]
        public async Task<BoardGame?> UpdateBoardGame([Service] ApplicationDbContext context, BoardGameDTO model)
        {
            var boardgame = await context.BoardGames.FirstOrDefaultAsync(b => b.Id == model.Id);
            if (boardgame != null)
            {
                if (!string.IsNullOrEmpty(model.Name))
                {
                    boardgame.Name = model.Name;
                }
                if (model.Year.HasValue && model.Year.Value > 0)
                {
                    boardgame.Year = model.Year.Value;
                }
                boardgame.LastModifiedDate = DateTime.Now;
                context.BoardGames.Update(boardgame);
                await context.SaveChangesAsync();
            }
            return boardgame;
        }

        [Serial]
        [Authorize(Roles = new[] { RoleNames.Administrator })]
        public async Task DeleteBoardGames([Service] ApplicationDbContext context, BoardGameDTO model)
        {
            var boardgame = await context.BoardGames.FirstOrDefaultAsync(b => b.Id == model.Id);
            if (boardgame != null)
            {
                context.BoardGames.Remove(boardgame);
                await context.SaveChangesAsync();
            }
        }

        //DOMAIN MUTATIONS
        [Serial]
        [Authorize(Roles = new[] { RoleNames.Moderator })]
        public async Task<Domain?> UpdateDomain([Service] ApplicationDbContext context, DomainDTO model)
        {
            var domain = await context.Domains.FirstOrDefaultAsync(d => d.Id == model.Id);
            if (domain != null)
            {
                if (!string.IsNullOrEmpty(model.Name))
                {
                    domain.Name = model.Name;
                }
                domain.LastModifiedDate = DateTime.Now;
                context.Domains.Update(domain);
                await context.SaveChangesAsync();
            }
            return domain;

        }

        [Serial]
        [Authorize(Roles = new[] { RoleNames.Administrator })]
        public async Task DeleteDomain([Service] ApplicationDbContext context, DomainDTO model)
        {
            var domain = await context.Domains.FirstOrDefaultAsync(d => d.Id == model.Id);

            if (domain != null)
            {
                context.Domains.Remove(domain);
                await context.SaveChangesAsync();
            }
        }


        //MECHANIC MUTATIONS
        [Serial]
        [Authorize(Roles = new[] { RoleNames.Moderator })]
        public async Task<Mechanic?> UpdateMechanic([Service] ApplicationDbContext context, MechanicDTO model)
        {
            var mechanic = await context.Mechanics.FirstOrDefaultAsync(m => m.Id == model.Id);
            if (mechanic != null)
            {
                if (string.IsNullOrEmpty(model.Name))
                {
                    mechanic.Name = model.Name;
                }
                mechanic.LastModifiedDate = DateTime.Now;
                context.Mechanics.Update(mechanic);
                await context.SaveChangesAsync();
            }
            return mechanic;
        }

        [Serial]
        [Authorize(Roles = new[] { RoleNames.Administrator })]
        public async Task DeleteMechanic([Service] ApplicationDbContext context, MechanicDTO model)
        {
            var mechanic = await context.Mechanics.FirstOrDefaultAsync(mec => mec.Id == model.Id);
            if (mechanic != null)
            {
                context.Mechanics.Remove(mechanic);
                await context.SaveChangesAsync();
            }
        }
    }
}
