namespace NOOBS_CMDR.Commands
{
    public class CommandType
    {

        #region Enums

        public enum Type
        {
            Stream,
            Record,
            Profile,
            Scene,
            Source,
            Filter,
            Audio,
            Media,
            Transition,
            StudioMode,
            Screenshot,
            ReplayBuffer,
            Projector,
            TriggerHotkey,
            Delay,
            Custom
        }

        #endregion Enums

        #region Properties

        public Type commandTypeID { get; set; }
        public string commandTypeDesc { get; set; }

        #endregion Properties

        #region Constructors

        public CommandType(string commandTypeDesc, Type commandTypeID)
        {
            this.commandTypeDesc = commandTypeDesc;
            this.commandTypeID = commandTypeID;
        }

        #endregion Constructors

    }

}
