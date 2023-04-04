using pdipadapter.Models;

namespace pdipadapter.Features.Players;

public interface IPlayersService
{
    Task<IEnumerable<Player>> GetPlayersList();
    Task<Player> GetPlayerById(int id);
    Task<Player> CreatePlayer(Player player);
    Task<Player> UpdatePlayer(Player player);
    Task<int> DeletePlayer(Player player);
}