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
    public partial class SourceControl : UserControl
    {
        public static readonly DependencyProperty commandProperty = DependencyProperty.Register("Command", typeof(SourceCommand), typeof(SourceControl));

        public SourceCommand Command
        {
            get { return (SourceCommand)GetValue(commandProperty); }
            set { SetValue(commandProperty, value); }
        }

        public SourceControl()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                SourceStateComboSetup();
                SceneComboSetup();
                SourceComboSetup();
            };
        }

        private void SourceStateComboSetup()
        {
            this.SourceStateCombo.SelectedValuePath = "Value";
            this.SourceStateCombo.DisplayMemberPath = "Key";

            SourceStateCombo.Items.Add(new KeyValuePair<string, SourceCommand.State>("Show Source", SourceCommand.State.Show));
            SourceStateCombo.Items.Add(new KeyValuePair<string, SourceCommand.State>("Hide Source", SourceCommand.State.Hide));
            SourceStateCombo.Items.Add(new KeyValuePair<string, SourceCommand.State>("Toggle Source", SourceCommand.State.Toggle));
        }

        private void RefreshScenes()
        {
            SceneCombo.ItemsSource = Command.obs.GetScenes(); ;
        }

        private void RefreshSources()
        {
            SourceCombo.ItemsSource = Command.obs.GetSources(Command.sceneName);
            SourceCombo.autoTextBox.Text = "";
        }

        private void SceneComboSetup()
        {
            RefreshScenes();
            SceneCombo.HideSuggestionListBox();
        }

        private void SourceComboSetup()
        {
            SourceCombo.HideSuggestionListBox();
        }

        private void SceneCombo_TextChanged(object sender, EventArgs e)
        {
            RefreshSources();
            SourceCombo.HideSuggestionListBox();
        }

        private void SceneCombo_TextBox_Clicked(object sender, EventArgs e)
        {
            RefreshScenes();
            SourceCombo.HideSuggestionListBox();
        }

        private void SourceCombo_TextBox_Clicked(object sender, EventArgs e)
        {
            RefreshSources();
            SceneCombo.HideSuggestionListBox();
        }
    }
}
