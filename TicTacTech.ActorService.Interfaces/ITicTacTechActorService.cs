using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace TicTacTech.ActorService.Interfaces
{
    public interface ITicTacTechActorService : IActor
    {
        Task<int> GetCountAsync();

        Task SetCountAsync(int count);
    }
}
