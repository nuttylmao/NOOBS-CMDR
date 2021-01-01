using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOOBS_CMDR.Commands
{
    public class StreamCommand : Command, INotifyPropertyChanged
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
            Toggle
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
                    OnPropertyChanged("streamingStatus");
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
