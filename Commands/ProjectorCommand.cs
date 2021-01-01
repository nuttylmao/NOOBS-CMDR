using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOOBS_CMDR.Commands
{
    public class ProjectorCommand : Command, INotifyPropertyChanged
    {

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        #region Enums

        public enum Type
        {
            Preview,
            Source,
            Scene,
            StudioProgram,
            MultiView
        }

        #endregion Enums

        #region Properties

        private Type _projectorType;
        public Type projectorType
        {
            get { return _projectorType; }
            set
            {
                if (value != _projectorType)
                {
                    _projectorType = value;
                    OnPropertyChanged("projectorType");
                }
            }
        }

        private bool _isFullscreen;
        public bool isFullscreen
        {
            get { return _isFullscreen; }
            set
            {
                if (value != _isFullscreen)
                {
                    _isFullscreen = value;
                    OnPropertyChanged("isFullscreen");
                }
            }
        }

        private int? _monitor;
        public int? monitor
        {
            get { return _monitor; }
            set
            {
                if (value != _monitor)
                {
                    _monitor = value;
                    OnPropertyChanged("monitor");
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

        public ProjectorCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
            monitor = 0;
        }

        #endregion Constructors

        #region Functions

        public override string ToString()
        {
            string monitorCommand = "";
            string nameCommand = "";

            if (isFullscreen)
                monitorCommand = string.Format(@",monitor={0}", monitor);

            switch (projectorType)
            {
                case (Type.Scene):
                    nameCommand = string.Format(@",name=""{0}""", sceneName);
                    break;
                case (Type.Source):
                    nameCommand = string.Format(@",name=""{0}""", sourceName);
                    break;
            }

            return string.Format(@"/command=OpenProjector,type={0}{1}{2}", projectorType, monitorCommand, nameCommand);
        }

        public override Command Clone()
        {
            return new ProjectorCommand(obs)
            {
                projectorType = projectorType,
                monitor = monitor,
                isFullscreen = isFullscreen,
                sceneName = sceneName,
                sourceName = sourceName
            };
        }

        #endregion Functions

    }

}
