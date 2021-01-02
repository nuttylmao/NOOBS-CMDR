using OBSWebsocketDotNet;

namespace NOOBS_CMDR.Commands
{
    public class TransitionCommand : Command
    {

        #region Enums

        public enum State
        {
            SetCurrentTransition,
            SetTransitionDuration,
            SetSceneTransitionOverride,
            RemoveTransitionOverride
        }

        #endregion Enums

        #region Properties

        private State _transitionState;
        public State transitionState
        {
            get { return _transitionState; }
            set
            {
                if (value != _transitionState)
                {
                    _transitionState = value;
                    OnPropertyChanged();
                }
            }
        }

        private int? _duration;
        public int? duration
        {
            get { return _duration; }
            set
            {
                if (value != _duration)
                {
                    _duration = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _transitionName;
        public string transitionName
        {
            get { return _transitionName; }
            set
            {
                if (value != _transitionName)
                {
                    _transitionName = value;
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

        #endregion Properties

        #region Constructors

        public TransitionCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
            duration = 300;
        }

        #endregion Constructors

        #region Functions

        public override string ToString()
        {
            switch (transitionState)
            {
                case (State.SetCurrentTransition):
                    return string.Format(@"/command=SetCurrentTransition,transition-name=""{0}""", transitionName);
                case (State.SetTransitionDuration):
                    return string.Format(@"/command=SetTransitionDuration,duration={0}", duration);
                case (State.SetSceneTransitionOverride):
                    return string.Format(@"/command=SetSceneTransitionOverride,sceneName=""{0}"",transitionName=""{1}"",transitionDuration={2}", sceneName, transitionName, duration);
                case (State.RemoveTransitionOverride):
                    return string.Format(@"/command=RemoveSceneTransitionOverride,sceneName=""{0}""", sceneName);
                default:
                    return @"";
            }
        }

        public override Command Clone()
        {
            return new TransitionCommand(obs)
            {
                transitionState = transitionState,
                transitionName = transitionName,
                duration = duration,
                sceneName = sceneName
            };
        }

        #endregion Functions

    }

}
