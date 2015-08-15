using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TicTacTech.ActorService.Interfaces;

namespace TicTacTech.ActorService
{
    [DataContract]
    public class GameManagerState
    {
        [DataMember]
        public string Initiator;
    }

    public class GameManager : Actor<GameManagerState>, IGameManager
    {
        public static IGameManager Instance()
        {
            return ActorProxy.Create<IGameManager>(new ActorId("TicTacTechMgr"));
        }

        public async Task LetMePlay(IPlayer player)
        {
            var playerId = Player.GetId(player);

            if (State.Initiator == null)
            {
                State.Initiator = playerId;
                await Task.FromResult(true);
            }
            else
            {
                var initiator = State.Initiator;
                State.Initiator = null;
                await Game.CreateNew()
                    .StartGame(
                        Player.FromId(initiator),
                        Player.FromId(playerId));
            }
        }
    }

    public class Player : Actor, IPlayer
    {
        public static IPlayer FromId(string playerId)
        {
            return ActorProxy.Create<IPlayer>(new ActorId(playerId));
        }

        public static string GetId(IPlayer player)
        {
            return player.GetActorId().GetStringId();
        }

        public Task EnterGame(IGame game, IPlayer partner)
        {
            var evt = GetEvent<IPlayerEvents>();
            evt.GameStarted(partner.GetActorId().GetStringId());
            return Task.FromResult(true);
        }

        public Task GameStateChanged(Interfaces.GameState gameState, bool yourTurn)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetName()
        {
            return Task.FromResult(this.GetActorId().GetStringId());
        }

        public Task GoAndPlay()
        {
            GameManager.Instance().LetMePlay(this);
            return Task.FromResult(true);
        }

        public Task SelectCell(int cellId)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public class GameState
    {
        [DataMember]
        public string GameStateSerialized;
    }

    public class Game : Actor<GameState>, IGame
    {
        public static IGame CreateNew()
        {
            return ActorProxy.Create<IGame>(new ActorId(Guid.NewGuid()));
        }

        public Task MakeMove(int cellId, IPlayer player)
        {
            throw new NotImplementedException();
        }

        public async Task StartGame(IPlayer playerX, IPlayer playerO)
        {
            var t1 = playerX.EnterGame(this, playerO);
            var t2 = playerO.EnterGame(this, playerX);
            await Task.WhenAll(t1, t2);
            //return Task.FromResult(true);
        }

    }
}
