using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOOBS_CMDR.Commands
{
    public class DelayCommand : Command, INotifyPropertyChanged
    {

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        #region Properties

        private int _delay;
        public int delay
        {
            get { return _delay; }
            set
            {
                if (value != _delay)
                {
                    _delay = value;
                    OnPropertyChanged("delay");
                }
            }
        }

        #endregion Properties

        #region Constructors

        public DelayCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
            this.delay = 0;
        }

        #endregion Constructors

        #region Functions

        public override string ToString()
        {
            decimal delayInSec = (decimal)delay / 1000;

            return string.Format(@"/delay={0}", + delayInSec);
        }

        public override Command Clone()
        {
            return new DelayCommand(obs)
            {
                delay = delay
            };
        }

        #endregion Functions

    }

}
