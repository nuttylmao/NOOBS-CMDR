using NOOBS_CMDR.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NOOBS_CMDR.Controls.Commands
{
    /// <summary>
    /// Interaction logic for ActionControl.xaml
    /// </summary>
    public partial class StudioModeControl : UserControl
    {
        public static readonly DependencyProperty commandProperty = DependencyProperty.Register("Command", typeof(StreamCommand), typeof(StudioModeControl));

        public StudioModeCommand Command
        {
            get { return (StudioModeCommand)GetValue(commandProperty); }
            set { SetValue(commandProperty, value); }
        }

        public StudioModeControl()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                StudioModeStateComboSetup();
            }; 
        }

        private void StudioModeStateComboSetup()
        {
            this.StudioModeStateCombo.SelectedValuePath = "Value";
            this.StudioModeStateCombo.DisplayMemberPath = "Key";

            StudioModeStateCombo.Items.Add(new KeyValuePair<string, StudioModeCommand.State>("Enable Studio Mode", StudioModeCommand.State.Enable));
            StudioModeStateCombo.Items.Add(new KeyValuePair<string, StudioModeCommand.State>("Disable Studio Mode", StudioModeCommand.State.Disable));
            StudioModeStateCombo.Items.Add(new KeyValuePair<string, StudioModeCommand.State>("Toggle Studio Mode", StudioModeCommand.State.Toggle));
        }
    }
}
