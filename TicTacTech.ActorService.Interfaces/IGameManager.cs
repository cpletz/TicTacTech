using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacTech.ActorService.Interfaces
{

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

    public interface IPlayerEvents : IActorEvents
    {
        void GameStarted(string otherPlayer, string yourRole);
        void GameStateChanged(string cells, PlayerGameStatus status);
    }

    public interface IPlayer : IActor, IActorEventPublisher<IPlayerEvents>
    {
        Task<string> GetName();

        // called from UI
        Task GoAndPlay();
        Task SelectCell(int cellId);

        // called from Game
        Task EnterGame(IGame game, IPlayer partner, string role);
        Task GameStateChanged(string cells, PlayerGameStatus status);
    }

    public interface IGame : IActor
    {
        Task StartGame(IPlayer playerX, IPlayer playerO);
        Task MakeMove(int cellIdx, IPlayer player);
    }

    public interface IGameManager : IActor      
    {
        Task LetMePlay(IPlayer player);
    }
}
