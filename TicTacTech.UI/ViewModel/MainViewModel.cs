using Microsoft.ServiceFabric.Actors;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using TicTacTech.ActorService.Interfaces;
using System.Windows.Media;
using System.Windows;

namespace TicTacTech.UI.ViewModel
{
    public class MainViewModel : ViewModelBase, IPlayerEvents
    {

        public MainViewModel()
        {
            Cells = Enumerable.Range(0, 9).Select(x => new CellViewModel()).ToArray();
        }

        internal bool CanEnterGame() =>
            PlayerName != null &&
            PlayerName.Length > 0 &&
            ClientStatus != ClientStatus.MyTurn &&
            ClientStatus != ClientStatus.OthersTurn;

        internal async Task EnterGame()
        {
            ClientStatus = ClientStatus.WaitingForOther;
            NotifyStatusChanges();
            ResetCells();
            Player = ActorProxy.Create<IPlayer>(new ActorId(PlayerName), "fabric:/TicTacTech.ActorServiceApplication");
            await Player.SubscribeAsync(this);
            Player.GoAndPlay();
            PlayerStats = await Player.GetStats();
        }

        private void ResetCells()
        {
            foreach (var c in Cells) c.SetCell('E');
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
            ClientStatus = ClientStatus.NewGame;
            OtherPlayer = otherPlayer;
            Role = yourRole;
            NotifyStatusChanges();
        }

        void IPlayerEvents.GameStateChanged(string cells, PlayerGameStatus status)
        {
            UpdateCells(cells);
            UpdateClientStatus(status);

            if (status == PlayerGameStatus.Tie || status == PlayerGameStatus.YouLost || status == PlayerGameStatus.YouWon)
            {
                Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
                {
                    PlayerStats = await Player.GetStats();
                }, DispatcherPriority.ApplicationIdle);

                var message = status ==
                    PlayerGameStatus.Tie ?
                    "Noone won." :
                    status == PlayerGameStatus.YouLost ?
                    "Sorry, you lost." :
                    "Congratulations! You won.";

                MessageBox.Show(message, $"Hi {PlayerName}", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
            switch (status)
            {
                case PlayerGameStatus.MoveRequired:
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
            NotifyStatusChanges();
        }

        private void NotifyStatusChanges()
        {
            RaisePropertyChanged(nameof(ClientStatus));
            RaisePropertyChanged(nameof(Status));
            RaisePropertyChanged(nameof(MyTurn));
            RaisePropertyChanged(nameof(BorderColor));
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

        public bool MyTurn => ClientStatus == ClientStatus.MyTurn;

        public Brush BorderColor => MyTurn ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.Red);

        public IPlayer Player { get; private set; }

        public PlayerStats PlayerStats
        {
            get { return _playerStats; }
            set
            {
                _playerStats = value;
                RaisePropertyChanged(nameof(PlayerStats));
            }
        }

        PlayerStats _playerStats;


    }
}
