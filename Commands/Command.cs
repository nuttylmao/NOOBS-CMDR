using OBSWebsocketDotNet;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NOOBS_CMDR.Commands
{
    public abstract class Command : INotifyPropertyChanged
    {

        #region Member Variables

        public OBSWebsocket obs { get; set; }

        #endregion Member Variables

        #region Constructors

        public Command(OBSWebsocket obs)
        {
            this.obs = obs;
        }

        #endregion Constructors

        #region Functions

        public abstract Command Clone();

        #endregion Functions

        #region INotifyPropertyChanged
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion INotifyPropertyChanged
    }

}
