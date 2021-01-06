using OBSWebsocketDotNet;

namespace NOOBS_CMDR.Commands
{
    public class RecordingCommand : Command
    {

        #region Enums

        public enum Status
        {
            Toggle,
            Start,
            Stop,
            Pause,
            Resume
        }

        #endregion Enums

        #region Properties

        private Status _recordingStatus;
        public Status recordingStatus
        {
            get { return _recordingStatus; }
            set
            {
                if (value != _recordingStatus)
                {
                    _recordingStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Properties

        #region Constructors

        public RecordingCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
        }

        #endregion Constructors

        #region Functions

        public override string ToString()
        {
            switch (recordingStatus)
            {
                case (Status.Start):
                    return @"/startrecording";
                case (Status.Stop):
                    return @"/stoprecording";
                case (Status.Toggle):
                    return @"/command=StartStopRecording";
                case (Status.Pause):
                    return @"/command=PauseRecording";
                case (Status.Resume):
                    return @"/command=ResumeRecording";
                default:
                    return @"";
            }
        }

        public override Command Clone()
        {
            return new RecordingCommand(obs)
            {
                recordingStatus = recordingStatus
            };
        }

        #endregion Functions

    }

}
