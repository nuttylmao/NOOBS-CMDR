using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOOBS_CMDR.Commands
{
    public class CustomCommand : Command, INotifyPropertyChanged
    {

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        #region Properties

        private string _command;
        public string command
        {
            get { return _command; }
            set
            {
                if (value != _command)
                {
                    _command = value;
                    OnPropertyChanged("command");
                }
            }
        }

        #endregion Properties

        #region Constructors

        public CustomCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
        }

        #endregion Constructors

        #region Functions

        public override string ToString()
        {
            return command;
        }

        public override Command Clone()
        {
            return new CustomCommand(obs)
            {
                command = command
            };
        }

        #endregion Functions

    }

}
