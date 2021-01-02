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
    public partial class SceneControl : UserControl
    {
        public static readonly DependencyProperty commandProperty = DependencyProperty.Register("Command", typeof(SceneCommand), typeof(SceneControl));

        public SceneCommand Command
        {
            get { return (SceneCommand)GetValue(commandProperty); }
            set { SetValue(commandProperty, value); }
        }

        public SceneControl()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                SceneComboSetup();
            };
        }

        private void RefreshScenes()
        {
            if (!Command.obs.IsConnected)
            {
                SceneCombo.ItemsSource = null;
                return;
            }

            List<string> scenes = Command.obs.GetSceneList().Scenes.ConvertAll(x => x.Name);
            SceneCombo.ItemsSource = Command.obs.GetScenes();
        }

        private void SceneComboSetup()
        {
            RefreshScenes();
            SceneCombo.HideSuggestionListBox();
        }

        private void SceneCombo_AutoTextBox_Clicked(object sender, EventArgs e)
        {
            RefreshScenes();
        }
    }
}
