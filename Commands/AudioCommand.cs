using OBSWebsocketDotNet;

namespace NOOBS_CMDR.Commands
{
    public class AudioCommand : Command
    {

        #region Enums

        public enum State
        {
            toggle,
            mute,
            unmute,
            setVolume,
            setMonitoringType
        }

        public enum MonitoringType
        {
            none,
            monitorOnly,
            monitorAndOutput
        }

        #endregion Enums

        #region Properties

        private State _audioState;
        public State audioState
        {
            get { return _audioState; }
            set
            {
                if (value != _audioState)
                {
                    _audioState = value;
                    OnPropertyChanged();
                }
            }
        }

        private MonitoringType _monitoringType;
        public MonitoringType monitoringType
        {
            get { return _monitoringType; }
            set
            {
                if (value != _monitoringType)
                {
                    _monitoringType = value;
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

        private int _volume;
        public int volume
        {
            get { return _volume; }
            set
            {
                if (value != _volume)
                {
                    _volume = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _delay;
        public int delay
        {
            get { return _delay; }
            set
            {
                if (value != _delay)
                {
                    _delay = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Properties

        #region Constructors

        public AudioCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
            volume = 100;
            delay = 10;
        }

        #endregion Constructors

        #region Functions

        public override string ToString()
        {
            switch (audioState)
            {
                case (State.mute):
                    return $@"/mute=""{sourceName}""";
                case (State.unmute):
                    return $@"/unmute=""{sourceName}""";
                case (State.toggle):
                    return $@"/toggleaudio=""{sourceName}""";
                case (State.setVolume):
                    return $@"/setvolume=""{sourceName}"",{volume},{delay}";
                case (State.setMonitoringType):
                    return $@"/command=SetAudioMonitorType,sourceName=""{sourceName}"",monitorType={monitoringType}";
                default:
                    return @"";
            }
        }

        public override Command Clone()
        {
            return new AudioCommand(obs)
            {
                audioState = audioState
            };
        }

        #endregion Functions

    }

}
