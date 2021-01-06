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
    public partial class RecordingControl : UserControl
    {
        public static readonly DependencyProperty commandProperty = DependencyProperty.Register("Command", typeof(RecordingCommand), typeof(RecordingControl));

        public RecordingCommand Command
        {
            get { return (RecordingCommand)GetValue(commandProperty); }
            set { SetValue(commandProperty, value); }
        }
        public RecordingControl()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                RecordingStatusComboSetup();
            };
        }

        private void RecordingStatusComboSetup()
        {
            this.RecordingStatusCombo.SelectedValuePath = "Value";
            this.RecordingStatusCombo.DisplayMemberPath = "Key";

            RecordingStatusCombo.Items.Add(new KeyValuePair<string, RecordingCommand.Status>("Toggle Recording", RecordingCommand.Status.Toggle));
            RecordingStatusCombo.Items.Add(new KeyValuePair<string, RecordingCommand.Status>("Start Recording", RecordingCommand.Status.Start));
            RecordingStatusCombo.Items.Add(new KeyValuePair<string, RecordingCommand.Status>("Stop Recording", RecordingCommand.Status.Stop));
            RecordingStatusCombo.Items.Add(new KeyValuePair<string, RecordingCommand.Status>("Pause Recording", RecordingCommand.Status.Pause));
            RecordingStatusCombo.Items.Add(new KeyValuePair<string, RecordingCommand.Status>("Resume Recording", RecordingCommand.Status.Resume));
        }
    }
}
