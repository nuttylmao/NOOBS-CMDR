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
            if (!Command.obs.IsConnected)
            {
                SourceCombo.ItemsSource = null;
                return;
            }

            List<string> sources = new List<string>();

            // Add scenes
            foreach (OBSScene scene in Command.obs.GetSceneList().Scenes)
            {
                sources.Add(scene.Name);
            }

            // Add sources
            foreach (OBSScene scene in Command.obs.GetSceneList().Scenes)
            {
                foreach (SceneItem source in scene.Items)
                {
                    if (!sources.Contains(source.SourceName))
                        sources.Add(source.SourceName);
                }
            }

            sources.Sort();

            SourceCombo.ItemsSource = sources;
        }

        private void RefreshFilters()
        {
            if (!Command.obs.IsConnected)
            {
                FilterCombo.ItemsSource = null;
                return;
            }

            List<string> sources = new List<string>();

            List<FilterSettings> filters = new List<FilterSettings>();

            try
            {
                filters = Command.obs.GetSourceFilters(Command.sourceName);
            }
            //catch (Exception ex)
            catch
            {
                filters = new List<FilterSettings>();
            }
            finally
            {
                foreach (FilterSettings filter in filters)
                {
                    sources.Add(filter.Name);
                }

                FilterCombo.ItemsSource = sources;
                FilterCombo.autoTextBox.Text = "";
            }
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
        }

        private void SourceCombo_AutoTextBox_Clicked(object sender, EventArgs e)
        {
            FilterCombo.HideSuggestionListBox();
        }

        private void FilterCombo_AutoTextBox_Clicked(object sender, EventArgs e)
        {
            SourceCombo.HideSuggestionListBox();
        }
    }
}
