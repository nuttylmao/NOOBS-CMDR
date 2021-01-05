using Microsoft.Win32;
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
    public partial class ScreenshotControl : UserControl
    {
        public static readonly DependencyProperty commandProperty = DependencyProperty.Register("Command", typeof(ScreenshotCommand), typeof(ScreenshotControl));

        public ScreenshotCommand Command
        {
            get { return (ScreenshotCommand)GetValue(commandProperty); }
            set { SetValue(commandProperty, value); }
        }

        public ScreenshotControl()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                ScreenshotTypeComboSetup();
                SceneComboSetup();
                SourceComboSetup();
                CreateNewComboSetup();
            };
        }

        private void ScreenshotTypeComboSetup()
        {
            this.ScreenshotTypeCombo.SelectedValuePath = "Value";
            this.ScreenshotTypeCombo.DisplayMemberPath = "Key";

            ScreenshotTypeCombo.Items.Add(new KeyValuePair<string, ScreenshotCommand.Type>("Scene", ScreenshotCommand.Type.Scene));
            ScreenshotTypeCombo.Items.Add(new KeyValuePair<string, ScreenshotCommand.Type>("Source", ScreenshotCommand.Type.Source));
        }

        private void RefreshScenes()
        {
            SceneCombo.ItemsSource = Command.obs.GetScenes();
        }

        private void RefreshSources()
        {
            SourceCombo.ItemsSource = Command.obs.GetSources(null, true);
        }

        private void CreateNewComboSetup()
        {
            this.CreateNewCombo.SelectedValuePath = "Value";
            this.CreateNewCombo.DisplayMemberPath = "Key";

            CreateNewCombo.Items.Add(new KeyValuePair<string, bool>("Always Create New File", true));
            CreateNewCombo.Items.Add(new KeyValuePair<string, bool>("Overwrite Existing File", false));
        }

        private void SceneComboSetup()
        {
            RefreshScenes();
            SceneCombo.HideSuggestionListBox();
        }
        private void SourceComboSetup()
        {
            RefreshSources();
            SourceCombo.HideSuggestionListBox();
        }
        private void DirectoryField_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Image(*.png)|*.png";
            if (dlg.ShowDialog() == true)
            {
                Command.saveToFilePath = dlg.FileName.Replace("\\", "/");
            }
        }

        private void SceneCombo_TextBox_Clicked(object sender, EventArgs e)
        {
            RefreshScenes();
        }

        private void SourceCombo_TextBox_Clicked(object sender, EventArgs e)
        {
            RefreshSources();
        }
    }

    public class Screenshot_SceneVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((ScreenshotCommand.Type)value)
            {
                case ScreenshotCommand.Type.Scene:
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

    public class Screenshot_SourceVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((ScreenshotCommand.Type)value)
            {
                case ScreenshotCommand.Type.Source:
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
