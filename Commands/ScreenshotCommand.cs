using OBSWebsocketDotNet;

namespace NOOBS_CMDR.Commands
{
    public class ScreenshotCommand : Command
    {

        #region Enums

        public enum Type
        {
            Scene,
            Source
        }

        #endregion Enums

        #region Properties

        private Type _screenshotType;
        public Type screenshotType
        {
            get { return _screenshotType; }
            set
            {
                if (value != _screenshotType)
                {
                    _screenshotType = value;
                    OnPropertyChanged();
                }
            }
        }

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

        private string _sourceName;
        public string sourceName
        {
            get { return _sourceName; }
            set
            {
                if (value != _sourceName)
                {
                    _sourceName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _saveToFilePath;
        public string saveToFilePath
        {
            get { return _saveToFilePath; }
            set
            {
                if (value != _saveToFilePath)
                {
                    _saveToFilePath = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Properties

        #region Constructors

        public ScreenshotCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
        }

        #endregion Constructors

        #region Functions

        public override string ToString()
        {
            return string.Format(@"/command=TakeSourceScreenshot,sourceName=""{0}"",embedPictureFormat=""png"",saveToFilePath=""{1}""", screenshotType == Type.Scene ? sceneName : sourceName, saveToFilePath);
        }

        public override Command Clone()
        {
            return new ScreenshotCommand(obs)
            {
                screenshotType = screenshotType,
                sceneName = sceneName,
                sourceName = sourceName,
                saveToFilePath = saveToFilePath
            };
        }

        #endregion Functions

    }

}
