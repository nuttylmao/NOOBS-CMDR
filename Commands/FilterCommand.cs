using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOOBS_CMDR.Commands
{
    public class FilterCommand : Command, INotifyPropertyChanged
    {

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        #region Enums

        public enum State
        {
            Show,
            Hide
        }

        private State _filterState;
        public State filterState
        {
            get { return _filterState; }
            set
            {
                if (value != _filterState)
                {
                    _filterState = value;
                    OnPropertyChanged("filterState");
                }
            }
        }

        private string _sourceName;
        public string sourceName
        {
            get { return _sourceName; }
            set
            {
                if (value != _sourceName)
                {
                    _sourceName = value;
                    OnPropertyChanged("sourceName");
                }
            }
        }

        private string _filterName;
        public string filterName
        {
            get { return _filterName; }
            set
            {
                if (value != _filterName)
                {
                    _filterName = value;
                    OnPropertyChanged("filterName");
                }
            }
        }

        #endregion Enums

        #region Constructors

        public FilterCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
        }

        #endregion Constructors

        #region Functions

        public override string ToString()
        {
            return string.Format(@"/command=SetSourceFilterVisibility,sourceName=""{0}"",filterName=""{1}"",filterEnabled={2}", sourceName, filterName, filterState == State.Show ? "True" : "False");
        }

        public override Command Clone()
        {
            return new FilterCommand(obs)
            {
                filterState = filterState,
                filterName = filterName,
                sourceName = sourceName
            };
        }

        #endregion Functions

    }

}
