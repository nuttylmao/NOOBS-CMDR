using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOOBS_CMDR.Commands
{
    public class RecordingCommand : Command, INotifyPropertyChanged
    {

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        #region Enums

        public enum Status
        {
            Start,
            Stop,
            Toggle,
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
                    OnPropertyChanged("recordingStatus");
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
