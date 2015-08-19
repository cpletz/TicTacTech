using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TicTacTech.UI.ViewModel;

namespace TicTacTech.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open,
                (s, e) => _mainViewModel.SelectCell(int.Parse(e.Parameter.ToString())),
                (s, e) => e.CanExecute = _mainViewModel?.CanSelectCell(int.Parse(e.Parameter.ToString())) ?? false));

            CommandBindings.Add(new CommandBinding(MediaCommands.Play,
                async (s, e) => await _mainViewModel.EnterGame(),
                (s, e) => e.CanExecute = _mainViewModel.CanEnterGame()));

        }
    }
}
