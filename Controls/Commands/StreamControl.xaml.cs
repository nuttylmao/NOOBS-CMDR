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
    public partial class StreamControl : UserControl
    {
        public static readonly DependencyProperty commandProperty = DependencyProperty.Register("Command", typeof(StreamCommand), typeof(StreamControl));

        public StreamCommand Command
        {
            get { return (StreamCommand)GetValue(commandProperty); }
            set { SetValue(commandProperty, value); }
        }

        public StreamControl()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                StreamingStatusComboSetup();
            }; 
        }

        private void StreamingStatusComboSetup()
        {
            this.StreamingStatusCombo.SelectedValuePath = "Value";
            this.StreamingStatusCombo.DisplayMemberPath = "Key";

            StreamingStatusCombo.Items.Add(new KeyValuePair<string, StreamCommand.Status>("Toggle Stream", StreamCommand.Status.Toggle));
            StreamingStatusCombo.Items.Add(new KeyValuePair<string, StreamCommand.Status>("Start Stream", StreamCommand.Status.Start));
            StreamingStatusCombo.Items.Add(new KeyValuePair<string, StreamCommand.Status>("Stop Stream", StreamCommand.Status.Stop));
        }
    }
}
