using Microsoft.ServiceFabric.Actors;
using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using TicTacTech.ActorService.Interfaces;

namespace TicTacTech.ActorService
{
    [DataContract]
    public class PlayerData
    {
        [DataMember]
        public Guid CurrentGameId { get; set; }

        [DataMember]
        public int Won { get; set; }

        [DataMember]
        public int Lost { get; set; }

        [DataMember]
        public int Ties { get; set; }
    }

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class Player : Actor<PlayerData>, IPlayer
    {

        public Task EnterGame(IGame game, IPlayer partner, string role)
        {
            State.CurrentGameId = Game.GetId(game);
            var evt = GetEvent<IPlayerEvents>();
            evt.GameStarted(partner.GetActorId().GetStringId(), role);
            return Task.FromResult(true);
        }

        public Task GameStateChanged(string cells, PlayerGameStatus status)
        {
            if (status == PlayerGameStatus.YouWon) State.Won++;
            else if (status == PlayerGameStatus.YouLost) State.Lost++;
            else if (status == PlayerGameStatus.Tie) State.Ties++;

            var evt = GetEvent<IPlayerEvents>();
            evt.GameStateChanged(cells, status);
            return Task.FromResult(true);
        }

        [Readonly]
        public Task<PlayerStats> GetStats()
        {
            return Task.FromResult(new PlayerStats { Won = State.Won, Lost = State.Lost, Ties = State.Ties });
        }

        [Readonly]
        public Task GoAndPlay()
        {
            GameManager.Instance().LetMePlay(this);
            return Task.FromResult(true);
        }

        [Readonly]
        public async Task SelectCell(int cellId)
        {
            var game = Game.FromId(State.CurrentGameId);
            await game.MakeMove(cellId, this);
        }

        public static IPlayer FromId(string playerId)
        {
            return ActorProxy.Create<IPlayer>(new ActorId(playerId));
        }

        public static string GetId(IPlayer player)
        {
            return player.GetActorId().GetStringId();
        }
    }
}
