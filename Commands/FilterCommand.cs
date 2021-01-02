using OBSWebsocketDotNet;

namespace NOOBS_CMDR.Commands
{
    public class FilterCommand : Command
    {

        #region Enums

        public enum State
        {
            Show,
            Hide
        }

        private State _filterState;
        public State filterState
        {
            get { return _filterState; }
            set
            {
                if (value != _filterState)
                {
                    _filterState = value;
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

        private string _filterName;
        public string filterName
        {
            get { return _filterName; }
            set
            {
                if (value != _filterName)
                {
                    _filterName = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Enums

        #region Constructors

        public FilterCommand(OBSWebsocket obs) : base(obs)
        {
            this.obs = obs;
        }

        #endregion Constructors

        #region Functions

        public override string ToString()
        {
            return string.Format(@"/command=SetSourceFilterVisibility,sourceName=""{0}"",filterName=""{1}"",filterEnabled={2}", sourceName, filterName, filterState == State.Show ? "True" : "False");
        }

        public override Command Clone()
        {
            return new FilterCommand(obs)
            {
                filterState = filterState,
                filterName = filterName,
                sourceName = sourceName
            };
        }

        #endregion Functions

    }

}
