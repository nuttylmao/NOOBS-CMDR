using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NOOBS_CMDR.Commands
{
    public class AudioCommand : Command, INotifyPropertyChanged
    {

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        #region Enums

        public enum State
        {
            mute,
            unmute,
            toggle,
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
                    OnPropertyChanged("audioState");
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
                    OnPropertyChanged("monitoringType");
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
                    OnPropertyChanged("sourceName");
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
                    OnPropertyChanged("volume");
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
                    OnPropertyChanged("delay");
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
                    return string.Format(@"/mute=""{0}""", sourceName);
                case (State.unmute):
                    return string.Format(@"/unmute=""{0}""", sourceName);
                case (State.toggle):
                    return string.Format(@"/toggleaudio=""{0}""", sourceName);
                case (State.setVolume):
                    return string.Format(@"/setvolume=""{0}"",{1},{2}", sourceName, volume, delay);
                case (State.setMonitoringType):
                    return string.Format(@"/command=SetAudioMonitorType,sourceName=""{0}"",monitorType={1}", sourceName, monitoringType);
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
