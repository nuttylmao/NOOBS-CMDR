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
    public partial class FilterControl : UserControl
    {
        public static readonly DependencyProperty commandProperty = DependencyProperty.Register("Command", typeof(FilterCommand), typeof(FilterControl));

        public FilterCommand Command
        {
            get { return (FilterCommand)GetValue(commandProperty); }
            set { SetValue(commandProperty, value); }
        }

        public FilterControl()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                FilterStateComboSetup();
                SourceComboSetup();
                FilterComboSetup();
            };
        }

        private void FilterStateComboSetup()
        {
            this.FilterStateCombo.SelectedValuePath = "Value";
            this.FilterStateCombo.DisplayMemberPath = "Key";

            FilterStateCombo.Items.Add(new KeyValuePair<string, FilterCommand.State>("Show Filter", FilterCommand.State.Show));
            FilterStateCombo.Items.Add(new KeyValuePair<string, FilterCommand.State>("Hide Filter", FilterCommand.State.Hide));
        }

        private void RefreshSources()
        {
            SourceCombo.ItemsSource = Command.obs.GetSources();
        }

        private void RefreshFilters()
        {
            FilterCombo.ItemsSource = Command.obs.GetFilters(Command.sourceName);
        }

        private void SourceComboSetup()
        {
            RefreshSources();
            SourceCombo.HideSuggestionListBox();
        }

        private void FilterComboSetup()
        {
           FilterCombo.HideSuggestionListBox();
        }

        private void SourceCombo_TextChanged(object sender, EventArgs e)
        {
            RefreshFilters();
            FilterCombo.HideSuggestionListBox();
            FilterCombo.autoTextBox.Text = "";
        }

        private void SourceCombo_TextBox_Clicked(object sender, EventArgs e)
        {
            RefreshSources();
            FilterCombo.HideSuggestionListBox();
        }

        private void FilterCombo_TextBox_Clicked(object sender, EventArgs e)
        {
            RefreshFilters();
            SourceCombo.HideSuggestionListBox();
        }
    }
}
