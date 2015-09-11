using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace TicTacTech.ActorService.Interfaces
{
    public interface IGameManager : IActor      
    {
        Task LetMePlay(IPlayer player);
    }
}
