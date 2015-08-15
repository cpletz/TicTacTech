using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using Microsoft.ServiceFabric.Actors;

namespace TicTacTech.ActorService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                using (FabricRuntime fabricRuntime = FabricRuntime.Create())
                {
                    fabricRuntime.RegisterActor(typeof(TicTacTechActorService));
                    fabricRuntime.RegisterActor(typeof(GameManager));
                    fabricRuntime.RegisterActor(typeof(Game));
                    fabricRuntime.RegisterActor(typeof(Player));

                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e);
                throw;
            }
        }
    }
}
