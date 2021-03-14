using OBSWebsocketDotNet;

namespace NOOBS_CMDR.Commands
{
    public class MediaCommand : Command
    {

        #region Enums

        public enum ControlType
        {
            play,
            pause,
            restart,
            stop,
            next,
            previous
        }

        #endregion Enums

        #region Properties

        private ControlType _controlType;
        public ControlType controlType
        {
            get { return _controlType; }
            set
            {
                if (value != _controlType)
                {
                    _controlType = value;
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

        #endregion Properties

        #region Constructors

        public MediaCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
        }

        #endregion Constructors

        #region Functions

        public override string ToString()
        {
            switch (controlType)
            {
                case (ControlType.play):
                    return $@"/command=PlayPauseMedia,sourceName=""{sourceName}"",playPause=false";
                case (ControlType.pause):
                    return $@"/command=PlayPauseMedia,sourceName=""{sourceName}"",playPause=true";
                case (ControlType.restart):
                    return $@"/command=RestartMedia,sourceName=""{sourceName}""";
                case (ControlType.stop):
                    return $@"/command=StopMedia,sourceName=""{sourceName}""";
                case (ControlType.next):
                    return $@"/command=NextMedia,sourceName=""{sourceName}""";
                case (ControlType.previous):
                    return $@"/command=PreviousMedia,sourceName=""{sourceName}""";
                default:
                    return @"";
            }
        }

        public override Command Clone()
        {
            return new MediaCommand(obs)
            {
                controlType = controlType,
                sourceName = sourceName
            };
        }

        #endregion Functions

    }

}
