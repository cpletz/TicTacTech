using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacTech.ActorService.Interfaces
{
    public class GameState
    {
        public string State { get; set; }
    }

    public interface IPlayerEvents : IActorEvents
    {
        void GameStarted(string partnerPlayerId);
        void GameStateChanged(GameState gameState, bool yourTurn);
    }

    public interface IPlayer : IActor, IActorEventPublisher<IPlayerEvents>
    {
        Task<string> GetName();

        // called from UI
        Task GoAndPlay();
        Task SelectCell(int cellId);

        // called from Game
        Task EnterGame(IGame game, IPlayer partner);
        Task GameStateChanged(GameState gameState, bool yourTurn);
    }

    public interface IGame : IActor
    {
        Task StartGame(IPlayer playerX, IPlayer playerO);
        Task MakeMove(int cellId, IPlayer player);
    }

    public interface IGameManager : IActor      
    {
        Task LetMePlay(IPlayer player);
    }
}
