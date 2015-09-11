using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace TicTacTech.ActorService.Interfaces
{

    public interface IPlayer : IActor, IActorEventPublisher<IPlayerEvents>
    {
        // called from UI
        Task GoAndPlay();
        Task SelectCell(int cellId);
        Task<PlayerStats> GetStats();

        // called from Game
        Task EnterGame(IGame game, IPlayer partner, string role);
        Task GameStateChanged(string cells, PlayerGameStatus status);
    }

public interface IPlayerEvents : IActorEvents
{
    void GameStarted(string otherPlayer, string yourRole);
    void GameStateChanged(string cells, PlayerGameStatus status);
}

    public enum PlayerGameStatus
    {
        WaitAndSee,
        MoveRequired,
        YouWon,
        YouLost,
        Tie
    }

    public class PlayerGameState
    {
        public string Cells { get; set; }
        public PlayerGameStatus Status { get; set; }
    }

    [DataContract]
    public class PlayerStats
    {
        [DataMember]
        public int Won { get; set; }
        [DataMember]
        public int Lost { get; set; }
        [DataMember]
        public int Ties { get; set; }
    }
}
