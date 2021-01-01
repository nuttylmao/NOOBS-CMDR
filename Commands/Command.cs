using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOOBS_CMDR.Commands
{
    public abstract class Command
    {

        #region Member Variables

        public OBSWebsocket obs { get; set; }

        #endregion Member Variables

        #region Constructors

        public Command(OBSWebsocket obs)
        {
            this.obs = obs;
        }

        #endregion Constructors

        #region Functions

        public abstract Command Clone();

        #endregion Functions

    }

}
