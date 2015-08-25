using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacTech.UI.ViewModel
{
    public enum ClientStatus
    {
        NoGame,
        NewGame,
        WaitingForOther,
        MyTurn,
        OthersTurn,
        Won,
        Lost,
        Tie
    }
}
