using OBSWebsocketDotNet;

namespace NOOBS_CMDR.Commands
{
    public class StreamCommand : Command
    {

        #region Enums

        public enum Status
        {
            Toggle,
            Start,
            Stop
        }

        #endregion Enums

        #region Properties

        private Status _streamingStatus { get; set; }
        public Status streamingStatus
        {
            get { return _streamingStatus; }
            set
            {
                if (value != _streamingStatus)
                {
                    _streamingStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Properties

        #region Constrcutors

        public StreamCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
        }

        #endregion Constrcutors

        #region Functions

        public override string ToString()
        {
            switch (streamingStatus)
            {
                case (Status.Start):
                    return @"/startstream";
                case (Status.Stop):
                    return @"/stopstream";
                case (Status.Toggle):
                    return @"/command=StartStopStreaming";
                default:
                    return @"";
            }
        }

        public override Command Clone()
        {
            return new StreamCommand(obs)
            {
                streamingStatus = streamingStatus
            };
        }

        #endregion Functions

    }

}
