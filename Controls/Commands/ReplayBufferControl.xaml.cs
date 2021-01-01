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
    public partial class ReplayBufferControl : UserControl
    {
        public static readonly DependencyProperty commandProperty = DependencyProperty.Register("Command", typeof(ReplayBufferCommand), typeof(ReplayBufferControl));

        public ReplayBufferCommand Command
        {
            get { return (ReplayBufferCommand)GetValue(commandProperty); }
            set { SetValue(commandProperty, value); }
        }

        public ReplayBufferControl()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                ReplayBufferStateComboSetup();
            }; 
        }

        private void ReplayBufferStateComboSetup()
        {
            this.ReplayBufferStateCombo.SelectedValuePath = "Value";
            this.ReplayBufferStateCombo.DisplayMemberPath = "Key";

            ReplayBufferStateCombo.Items.Add(new KeyValuePair<string, ReplayBufferCommand.State>("Start Replay Buffer", ReplayBufferCommand.State.Start));
            ReplayBufferStateCombo.Items.Add(new KeyValuePair<string, ReplayBufferCommand.State>("Stop Replay Buffer", ReplayBufferCommand.State.Stop));
            ReplayBufferStateCombo.Items.Add(new KeyValuePair<string, ReplayBufferCommand.State>("Toggle Replay Buffer", ReplayBufferCommand.State.Toggle));
            ReplayBufferStateCombo.Items.Add(new KeyValuePair<string, ReplayBufferCommand.State>("Save Replay Buffer", ReplayBufferCommand.State.Save));
        }
    }
}
