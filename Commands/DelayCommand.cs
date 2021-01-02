using OBSWebsocketDotNet;

namespace NOOBS_CMDR.Commands
{
    public class DelayCommand : Command
    {

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
                    OnPropertyChanged();
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
