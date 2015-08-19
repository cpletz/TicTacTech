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
    public class GameManagerData
    {
        [DataMember]
        public string Initiator;
    }

    public class GameManager : Actor<GameManagerData>, IGameManager
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




}
