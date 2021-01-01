using NOOBS_CMDR.Commands;
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
    public partial class TransitionControl : UserControl
    {
        public static readonly DependencyProperty commandProperty = DependencyProperty.Register("Command", typeof(TransitionCommand), typeof(TransitionControl));

        public TransitionCommand Command
        {
            get { return (TransitionCommand)GetValue(commandProperty); }
            set { SetValue(commandProperty, value); }
        }

        public TransitionControl()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                TransitionStateComboSetup();
                SceneComboSetup();
                TransitionComboSetup();
            };
        }

        private void TransitionStateComboSetup()
        {
            this.TransitionStateCombo.SelectedValuePath = "Value";
            this.TransitionStateCombo.DisplayMemberPath = "Key";

            TransitionStateCombo.Items.Add(new KeyValuePair<string, TransitionCommand.State>("Set Current Transition", TransitionCommand.State.SetCurrentTransition));
            TransitionStateCombo.Items.Add(new KeyValuePair<string, TransitionCommand.State>("Set Transition Duration", TransitionCommand.State.SetTransitionDuration));
            TransitionStateCombo.Items.Add(new KeyValuePair<string, TransitionCommand.State>("Set Scene Transition Override", TransitionCommand.State.SetSceneTransitionOverride));
            TransitionStateCombo.Items.Add(new KeyValuePair<string, TransitionCommand.State>("Remove Scene Transition Override", TransitionCommand.State.RemoveTransitionOverride));
        }

        private void RefreshScenes()
        {
            if (!Command.obs.IsConnected)
            {
                SceneCombo.ItemsSource = null;
                return;
            }

            List<string> scenes = Command.obs.GetSceneList().Scenes.ConvertAll(x => x.Name);
            SceneCombo.ItemsSource = scenes;
        }

        private void RefreshTransitions()
        {
            if (!Command.obs.IsConnected)
            {
                TransitionCombo.ItemsSource = null;
                return;
            }

            List<string> transitions = Command.obs.GetTransitionList().Transitions.ConvertAll(x => x.Name);
            TransitionCombo.ItemsSource = transitions;
        }

        private void SceneComboSetup()
        {
            RefreshScenes();
            SceneCombo.HideSuggestionListBox();
        }

        private void TransitionComboSetup()
        {
            RefreshTransitions();
            TransitionCombo.HideSuggestionListBox();
        }

        private void DurationField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }

    public class Transition_SceneVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((TransitionCommand.State)value)
            {
                case TransitionCommand.State.SetSceneTransitionOverride:
                    return Visibility.Visible;
                case TransitionCommand.State.RemoveTransitionOverride:
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

    public class Transition_TransitionVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((TransitionCommand.State)value)
            {
                case TransitionCommand.State.SetCurrentTransition:
                    return Visibility.Visible;
                case TransitionCommand.State.SetSceneTransitionOverride:
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

    public class Transition_DurationVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((TransitionCommand.State)value)
            {
                case TransitionCommand.State.SetTransitionDuration:
                    return Visibility.Visible;
                case TransitionCommand.State.SetSceneTransitionOverride:
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
