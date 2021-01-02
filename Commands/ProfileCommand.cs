using OBSWebsocketDotNet;

namespace NOOBS_CMDR.Commands
{
    public class ProfileCommand : Command
    {

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
                    OnPropertyChanged();
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
