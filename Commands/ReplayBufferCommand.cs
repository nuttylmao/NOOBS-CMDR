using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOOBS_CMDR.Commands
{
    public class ReplayBufferCommand : Command, INotifyPropertyChanged
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
                    OnPropertyChanged("replayBufferState");
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
