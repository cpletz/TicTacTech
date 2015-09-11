using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace TicTacTech.ActorService.Interfaces
{
    public interface IGame : IActor
    {
        Task StartGame(IPlayer playerX, IPlayer playerO);
        Task MakeMove(int cellIdx, IPlayer player);
    }
}
