using OBSWebsocketDotNet;

namespace NOOBS_CMDR.Commands
{
    public class SceneCommand : Command
    {

        #region Properties

        private string _sceneName;
        public string sceneName
        {
            get { return _sceneName; }
            set
            {
                if (value != _sceneName)
                {
                    _sceneName = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Properties

        #region Constructors

        public SceneCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
        }

        #endregion Constructors

        #region Functions

        public override string ToString()
        {
            return @"/scene=""" + sceneName + @"""";
        }

        public override Command Clone()
        {
            return new SceneCommand(obs)
            {
                sceneName = sceneName
            };
        }

        #endregion Functions

    }

}
