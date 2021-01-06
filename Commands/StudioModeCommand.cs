using OBSWebsocketDotNet;

namespace NOOBS_CMDR.Commands
{
    public class StudioModeCommand : Command
    {

        #region Enums

        public enum State
        {
            Toggle,
            Enable,
            Disable
        }

        #endregion Enums

        #region Properties

        private State _studioModeState { get; set; }
        public State studioModeState
        {
            get { return _studioModeState; }
            set
            {
                if (value != _studioModeState)
                {
                    _studioModeState = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Properties

        #region Constructors

        public StudioModeCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
        }

        #endregion Constructors

        #region Functions

        public override string ToString()
        {
            switch (studioModeState)
            {
                case (State.Enable):
                    return @"/command=EnableStudioMode";
                case (State.Disable):
                    return @"/command=DisableStudioMode";
                case (State.Toggle):
                    return @"/command=ToggleStudioMode";
                default:
                    return @"";
            }
        }

        public override Command Clone()
        {
            return new StudioModeCommand(obs)
            {
                studioModeState = studioModeState
            };
        }

        #endregion Functions

    }

}
