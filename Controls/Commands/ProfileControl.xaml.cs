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
    public partial class ProfileControl : UserControl
    {
        public static readonly DependencyProperty commandProperty = DependencyProperty.Register("Command", typeof(ProfileCommand), typeof(ProfileControl));

        public ProfileCommand Command
        {
            get { return (ProfileCommand)GetValue(commandProperty); }
            set { SetValue(commandProperty, value); }
        }

        public ProfileControl()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                ProfileComboSetup();
            };
        }

        private void RefreshProfiles()
        {
            ProfileCombo.ItemsSource = Command.obs.GetProfiles();
        }

        private void ProfileComboSetup()
        {
            List<string> Profiles = Command.obs.ListProfiles();
            ProfileCombo.ItemsSource = Profiles;
            ProfileCombo.HideSuggestionListBox();
        }

        private void ProfileCombo_TextBox_Clicked(object sender, EventArgs e)
        {
            RefreshProfiles();
        }
    }
}
