using NOOBS_CMDR.Commands;
using NOOBS_CMDR.Extensions;
using OBSWebsocketDotNet.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class MediaControl : UserControl
    {
        public static readonly DependencyProperty commandProperty = DependencyProperty.Register("Command", typeof(MediaCommand), typeof(MediaControl));

        public MediaCommand Command
        {
            get { return (MediaCommand)GetValue(commandProperty); }
            set { SetValue(commandProperty, value); }
        }

        public MediaControl()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                ControlTypeComboSetup();
                MediaSourceComboSetup();
            };
        }

        private void ControlTypeComboSetup()
        {
            this.ControlTypeCombo.SelectedValuePath = "Value";
            this.ControlTypeCombo.DisplayMemberPath = "Key";

            ControlTypeCombo.Items.Add(new KeyValuePair<string, MediaCommand.ControlType>("Play", MediaCommand.ControlType.play));
            ControlTypeCombo.Items.Add(new KeyValuePair<string, MediaCommand.ControlType>("Play/Pause", MediaCommand.ControlType.pause));
            ControlTypeCombo.Items.Add(new KeyValuePair<string, MediaCommand.ControlType>("Restart", MediaCommand.ControlType.restart));
            ControlTypeCombo.Items.Add(new KeyValuePair<string, MediaCommand.ControlType>("Stop", MediaCommand.ControlType.stop));
            ControlTypeCombo.Items.Add(new KeyValuePair<string, MediaCommand.ControlType>("Next", MediaCommand.ControlType.next));
            ControlTypeCombo.Items.Add(new KeyValuePair<string, MediaCommand.ControlType>("Previous", MediaCommand.ControlType.previous));
        }

        private void RefreshMediaSources()
        {
            MediaSourceCombo.ItemsSource = Command.obs.GetMediaSources();
        }

        private void MediaSourceComboSetup()
        {
            RefreshMediaSources();
            MediaSourceCombo.HideSuggestionListBox();
        }

        private void VolumeField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = !IsValid(((TextBox)sender).Text + e.Text);
        }
        public static bool IsValid(string str)
        {
            int i;
            return int.TryParse(str, out i) && i >= 1 && i <= 100;
        }

        private void MediaSourceCombo_TextBox_Clicked(object sender, EventArgs e)
        {
            RefreshMediaSources();
        }
    }
}
