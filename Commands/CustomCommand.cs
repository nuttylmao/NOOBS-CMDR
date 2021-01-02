using OBSWebsocketDotNet;

namespace NOOBS_CMDR.Commands
{
    public class CustomCommand : Command
    {

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
                    OnPropertyChanged();
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
