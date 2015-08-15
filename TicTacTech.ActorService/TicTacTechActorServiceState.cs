using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using TicTacTech.ActorService.Interfaces;
using Microsoft.ServiceFabric;
using Microsoft.ServiceFabric.Actors;

namespace TicTacTech.ActorService
{
    [DataContract]
    public class TicTacTechActorServiceState
    {
        [DataMember]
        public int Count;

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "TicTacTechActorServiceState[Count = {0}]", Count);
        }
    }
}