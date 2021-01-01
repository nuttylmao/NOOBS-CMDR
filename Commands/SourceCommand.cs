using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOOBS_CMDR.Commands
{
    public class SourceCommand : Command, INotifyPropertyChanged
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
            Show,
            Hide,
            Toggle
        }

        #endregion Enums

        #region Properties

        private State _sourceState;
        public State sourceState
        {
            get { return _sourceState; }
            set
            {
                if (value != _sourceState)
                {
                    _sourceState = value;
                    OnPropertyChanged("sourceState");
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
                    OnPropertyChanged("sceneName");
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

        #endregion Properties

        #region Constructors

        public SourceCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
        }

        #endregion Constructors

        #region Functions

        public override string ToString()
        {
            switch (sourceState)
            {
                case (State.Show):
                    return string.Format(@"/showsource=""{0}""/""{1}""", sceneName, sourceName);
                case (State.Hide):
                    return string.Format(@"/hidesource=""{0}""/""{1}""", sceneName, sourceName);
                case (State.Toggle):
                    return string.Format(@"/togglesource=""{0}""/""{1}""", sceneName, sourceName);
                default:
                    return @"";
            }
        }

        public override Command Clone()
        {
            return new SourceCommand(obs)
            {
                sourceState = sourceState,
                sceneName = sceneName,
                sourceName = sourceName
            };
        }

        #endregion Functions

    }

}
