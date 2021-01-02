using OBSWebsocketDotNet;

namespace NOOBS_CMDR.Commands
{
    public class ReplayBufferCommand : Command
    {

        #region Enums

        public enum State
        {
            Start,
            Stop,
            Toggle,
            Save
        }

        #endregion Enums

        #region Properties

        private State _replayBufferState { get; set; }
        public State replayBufferState
        {
            get { return _replayBufferState; }
            set
            {
                if (value != _replayBufferState)
                {
                    _replayBufferState = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Properties

        #region Constructors

        public ReplayBufferCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
        }

        #endregion Constructors

        #region Functions

        public override string ToString()
        {
            switch (replayBufferState)
            {
                case (State.Start):
                    return @"/command=StartReplayBuffer";
                case (State.Stop):
                    return @"/command=StopReplayBuffer";
                case (State.Toggle):
                    return @"/command=StartStopReplayBuffer";
                case (State.Save):
                    return @"/command=SaveReplayBuffer";
                default:
                    return @"";
            }
        }

        public override Command Clone()
        {
            return new ReplayBufferCommand(obs)
            {
                replayBufferState = replayBufferState
            };
        }

        #endregion Functions

    }

}
