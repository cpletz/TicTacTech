using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacTech.UI.ViewModel
{
    public class CellViewModel : ViewModelBase
    {
        string _playedBy;

        public string PlayedBy
        {
            get
            {
                return _playedBy;
            }

            set
            {
                _playedBy = value;
                RaisePropertyChanged(nameof(PlayedBy));
                RaisePropertyChanged(nameof(IsEmpty));
            }
        }

        public bool IsEmpty => string.IsNullOrEmpty(PlayedBy);
        //{
        //    get { return string.IsNullOrEmpty(PlayedBy); }
        //}

        internal void SetCell(char c)
        {
            if (c == 'X' || c == 'O') PlayedBy = c.ToString();
            else PlayedBy = null;
        }
    }
}
