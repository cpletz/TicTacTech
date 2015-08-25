using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TicTacTech.ActorService.Interfaces;
using static TicTacToe;
using static TicTacToeGameStateSerializer;

namespace TicTacTech.ActorService
{
    [DataContract]
    public class GameData
    {
        [DataMember]
        public string PlayerX { get; set; }

        [DataMember]
        public string PlayerO { get; set; }

        [DataMember]
        public SerGameState GameState { get; set; }

        [DataMember]
        public bool TimedOut { get; set; }
    }

    public class Game : Actor<GameData>, IGame, IRemindable
    {
        internal static IGame CreateNew() => ActorProxy.Create<IGame>(new ActorId(Guid.NewGuid()));
        internal static Guid GetId(IGame game) => game.GetActorId().GetGuidId();
        internal static IGame FromId(Guid id) => ActorProxy.Create<IGame>(new ActorId(id));

        TicTacToeDomain.GameState GetGameState() => deserialize(State.GameState);

        public async Task MakeMove(int cellId, IPlayer player)
        {
            var gameState = GetGameState();
            var cellPos = positionByIndex(cellId);
            var playerId = Player.GetId(player);
            var isPlayerX = playerId == State.PlayerX;
            var isPlayerO = playerId == State.PlayerO;

            if ((gameState.status.IsMoveRequiredByX && !isPlayerX) ||
                gameState.status.IsMoveRequiredByO && !isPlayerO)
            {
                throw new ApplicationException("Wrong player.");
            }

            var newGameState = makeMove(gameState, cellPos);
            var playerStatus = default(PlayerGameStatus);
            var otherPlayerStatus = default(PlayerGameStatus);

            var newStatus = newGameState.status;
            if (newStatus.IsMoveRequiredByO || newStatus.IsMoveRequiredByX)
            {
                playerStatus = PlayerGameStatus.WaitAndSee;
                otherPlayerStatus = PlayerGameStatus.MoveRequired;
            }
            else if (newStatus.IsWonByO || newStatus.IsWonByX)
            {
                playerStatus = PlayerGameStatus.YouWon;
                otherPlayerStatus = PlayerGameStatus.YouLost;
            }
            else
            {
                playerStatus = PlayerGameStatus.Tie;
                otherPlayerStatus = PlayerGameStatus.Tie;
            }

            State.GameState = serialize(newGameState);
            var otherPlayer = Player.FromId(isPlayerX ? State.PlayerO : State.PlayerX);

            await Task.WhenAll(
                player.GameStateChanged(State.GameState.board, playerStatus),
                otherPlayer.GameStateChanged(State.GameState.board, otherPlayerStatus)
                );

        }

        public async Task StartGame(IPlayer playerX, IPlayer playerO)
        {
            State.PlayerX = Player.GetId(playerX);
            State.PlayerO = Player.GetId(playerO);
            State.GameState = serialize(startGame());

            await Task.WhenAll(
                playerX.EnterGame(this, playerO, "X"),
                playerO.EnterGame(this, playerX, "O"),
                RegisterReminder("GameTimeout", null, TimeSpan.FromSeconds(15), TimeSpan.MaxValue, ActorReminderAttributes.None));

            playerX.GameStateChanged(State.GameState.board, PlayerGameStatus.MoveRequired);
        }

        public Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period)
        {
            State.TimedOut = true;
            var gameStatus = GetGameState().status;
            if (gameStatus.IsMoveRequiredByO || gameStatus.IsMoveRequiredByX)
            {
                var playerX = Player.FromId(State.PlayerX);
                var playerO = Player.FromId(State.PlayerO);
                var playerXStatus = gameStatus.IsMoveRequiredByO ? PlayerGameStatus.YouWon : PlayerGameStatus.YouLost;
                var playerOStatus = gameStatus.IsMoveRequiredByX ? PlayerGameStatus.YouWon : PlayerGameStatus.YouLost;
                playerX.GameStateChanged(State.GameState.board, playerXStatus);
                playerO.GameStateChanged(State.GameState.board, playerOStatus);
            }
            return Task.FromResult(true);
        }
    }
}
