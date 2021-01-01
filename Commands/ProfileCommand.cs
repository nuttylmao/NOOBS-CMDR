using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOOBS_CMDR.Commands
{
    public class ProfileCommand : Command, INotifyPropertyChanged
    {

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        #region Properties

        private string _profileName;
        public string profileName
        {
            get { return _profileName; }
            set
            {
                if (value != _profileName)
                {
                    _profileName = value;
                    OnPropertyChanged("profileName");
                }
            }
        }

        #endregion Properties

        #region Constructors

        public ProfileCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
        }

        #endregion Constructors

        #region Functions

        public override string ToString()
        {
            return string.Format(@"/profile=""{0}""", profileName);
        }

        public override Command Clone()
        {
            return new ProfileCommand(obs)
            {
                profileName  = profileName
            };
        }

        #endregion Functions

    }

}
