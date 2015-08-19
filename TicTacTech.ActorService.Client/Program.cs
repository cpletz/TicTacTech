using TicTacTech.ActorService.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace TicTacTech.ActorService.Client
{
    public class Program
    {

        class PlayerEventHander : IPlayerEvents
        {
            public string Name { get; set; }


            public void GameStarted(string otherPlayer, string yourRole)
            {
                throw new NotImplementedException();
            }

            public void GameStateChanged(string cells, PlayerGameStatus status)
            {
                throw new NotImplementedException();
            }

            //public void GameStarted(string partnerPlayerId)
            //{
            //    WriteLine($"{Name} is playing with {partnerPlayerId}");
            //}

            //public void GameStateChanged(PlayerGameState gameState, bool yourTurn)
            //{
            //    WriteLine($"Game startet for player {Name}");
            //}
        }

        public static void Main(string[] args)
        {
            var playerXEvtHandler = new PlayerEventHander { Name = "PlayerX" };
            var playerOEvtHandler = new PlayerEventHander { Name = "PlayerO" };

            var playerX = ActorProxy.Create<IPlayer>(new ActorId("PlayerX"), "fabric:/TicTacTech.ActorServiceApplication");
            var playerO = ActorProxy.Create<IPlayer>(new ActorId("PlayerO"), "fabric:/TicTacTech.ActorServiceApplication");

            playerX.SubscribeAsync(playerXEvtHandler).Wait();
            playerO.SubscribeAsync(playerOEvtHandler).Wait();

            var tX = playerX.GoAndPlay();
            var tY = playerO.GoAndPlay();


            ReadLine();
        }

        static void Original()
        {

            var proxy = ActorProxy.Create<ITicTacTechActorService>(ActorId.NewId(), "fabric:/TicTacTech.ActorServiceApplication");

            int count = 10;
            Console.WriteLine("Setting Count to in Actor {0}: {1}", proxy.GetActorId(), count);
            proxy.SetCountAsync(count).Wait();

            Console.WriteLine("Count from Actor {1}: {0}", proxy.GetActorId(), proxy.GetCountAsync().Result);
        }
    }
}
