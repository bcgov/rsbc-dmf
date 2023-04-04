using pdipadapter.Data;
using pdipadapter.Models;
using Microsoft.EntityFrameworkCore;

namespace pdipadapter.Features.Players
{
    public class PlayersService : IPlayersService
    {
        private readonly JumDbContext _context;
        public PlayersService(JumDbContext context)
        {
            _context = context;
        }

        public async Task<Player> CreatePlayer(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return player;
        }

        public async Task<int> DeletePlayer(Player player)
        {
            _context.Players.Remove(player);
            return await _context.SaveChangesAsync();
        }

        public async Task<Player> GetPlayerById(int id)
        {
            return await _context.Players.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Player>> GetPlayersList()
        {
            return await _context.Players.ToListAsync();
        }

        public Task<Player> UpdatePlayer(Player player)
        {
           _context.Players.Update(player);
            _context.SaveChanges();
            return Task.FromResult(player);
        }
    }
}
