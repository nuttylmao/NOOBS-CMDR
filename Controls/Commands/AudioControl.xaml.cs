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
    public partial class AudioControl : UserControl
    {
        public static readonly DependencyProperty commandProperty = DependencyProperty.Register("Command", typeof(AudioCommand), typeof(AudioControl));

        public AudioCommand Command
        {
            get { return (AudioCommand)GetValue(commandProperty); }
            set { SetValue(commandProperty, value); }
        }

        public AudioControl()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                AudioSourceStateComboSetup();
                MonitoringTypeComboSetup();
                AudioSourceComboSetup();
            };
        }

        private void AudioSourceStateComboSetup()
        {
            this.AudioStateCombo.SelectedValuePath = "Value";
            this.AudioStateCombo.DisplayMemberPath = "Key";

            AudioStateCombo.Items.Add(new KeyValuePair<string, AudioCommand.State>("Mute Audio", AudioCommand.State.mute));
            AudioStateCombo.Items.Add(new KeyValuePair<string, AudioCommand.State>("Unmute Audio", AudioCommand.State.unmute));
            AudioStateCombo.Items.Add(new KeyValuePair<string, AudioCommand.State>("Toggle Audio", AudioCommand.State.toggle));
            AudioStateCombo.Items.Add(new KeyValuePair<string, AudioCommand.State>("Set Volume", AudioCommand.State.setVolume));
            AudioStateCombo.Items.Add(new KeyValuePair<string, AudioCommand.State>("Set Audio Monitoring Type", AudioCommand.State.setMonitoringType));
        }

        private void MonitoringTypeComboSetup()
        {
            this.MonitoringTypeCombo.SelectedValuePath = "Value";
            this.MonitoringTypeCombo.DisplayMemberPath = "Key";

            MonitoringTypeCombo.Items.Add(new KeyValuePair<string, AudioCommand.MonitoringType>("None", AudioCommand.MonitoringType.none));
            MonitoringTypeCombo.Items.Add(new KeyValuePair<string, AudioCommand.MonitoringType>("Monitor Only", AudioCommand.MonitoringType.monitorOnly));
            MonitoringTypeCombo.Items.Add(new KeyValuePair<string, AudioCommand.MonitoringType>("Monitor And Output", AudioCommand.MonitoringType.monitorAndOutput));
        }

        private void RefreshAudioSources()
        {
            AudioSourceCombo.ItemsSource = Command.obs.GetAudioSources();
        }

        private void AudioSourceComboSetup()
        {
            RefreshAudioSources();
            AudioSourceCombo.HideSuggestionListBox();
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
    }

    public class Audio_SetVolumeVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((AudioCommand.State)value)
            {
                case AudioCommand.State.setVolume:
                    return Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Audio_SetMonitoringTypeVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((AudioCommand.State)value)
            {
                case AudioCommand.State.setMonitoringType:
                    return Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
