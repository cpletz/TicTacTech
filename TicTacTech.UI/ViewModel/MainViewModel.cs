using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacTech.ActorService.Interfaces;

namespace TicTacTech.UI.ViewModel
{
    public class MainViewModel : ViewModelBase, IPlayerEvents
    {

        public MainViewModel()
        {
            Cells = Enumerable.Range(0, 9).Select(x => new CellViewModel()).ToArray();
        }

        internal bool CanEnterGame() =>
            ClientStatus != ClientStatus.MyTurn ||
            ClientStatus != ClientStatus.OthersTurn;


        internal async Task EnterGame()
        {
            Player = ActorProxy.Create<IPlayer>(new ActorId(PlayerName), "fabric:/TicTacTech.ActorServiceApplication");
            await Player.SubscribeAsync(this);
            Player.GoAndPlay();
        }

        public CellViewModel[] Cells { get; private set; }

        internal bool CanSelectCell(int idx) => MyTurn && Cells[idx].IsEmpty;

        internal void SelectCell(int idx)
        {
            Cells[idx].PlayedBy = Role;
            Player.SelectCell(idx);
        }

        void IPlayerEvents.GameStarted(string otherPlayer, string yourRole)
        {
            OtherPlayer = otherPlayer;
            Role = yourRole;
        }

        void IPlayerEvents.GameStateChanged(string cells, PlayerGameStatus status)
        {
            UpdateCells(cells);
            UpdateClientStatus(status);
        }



        private void UpdateCells(string cells)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Cells[i].SetCell(cells[i]);
            }
        }

        private void UpdateClientStatus(PlayerGameStatus status)
        {
            switch (status) {
                case PlayerGameStatus.MoveRequired:
                    MyTurn = true;
                    ClientStatus = ClientStatus.MyTurn;
                    break;
                case PlayerGameStatus.Tie:
                    ClientStatus = ClientStatus.Tie;
                    break;
                case PlayerGameStatus.YouLost:
                    ClientStatus = ClientStatus.Lost;
                    break;
                case PlayerGameStatus.YouWon:
                    ClientStatus = ClientStatus.Won;
                    break;
                default:
                    ClientStatus = ClientStatus.OthersTurn;
                    break;
            }
            RaisePropertyChanged(nameof(ClientStatus));
            RaisePropertyChanged(nameof(Status));
        }

        public ClientStatus ClientStatus { get; private set; } = ClientStatus.NoGame;

        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                _playerName = value;
                RaisePropertyChanged(nameof(PlayerName));
            }
        }
        string _playerName;

        public string OtherPlayer
        {
            get { return _otherPlayer; }
            set
            {
                _otherPlayer = value;
                RaisePropertyChanged(nameof(OtherPlayer));
            }
        }
        string _otherPlayer;

        public string Role
        {
            get { return _role; }
            set
            {
                _role = value;
                RaisePropertyChanged(nameof(Role));
            }
        }
        string _role;

        public string Status
        {
            get { return ClientStatus.ToString(); }
        }

        public bool MyTurn
        {
            get { return _myTurn; }
            set
            {
                _myTurn = value;
                RaisePropertyChanged(nameof(MyTurn));
            }
        }

        public IPlayer Player { get; private set; }

        bool _myTurn;

    }
}
