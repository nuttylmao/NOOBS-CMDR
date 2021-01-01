using Microsoft.Win32;
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
            if (!Command.obs.IsConnected)
            {
                SceneCombo.ItemsSource = null;
                return;
            }

            List<string> scenes = Command.obs.GetSceneList().Scenes.ConvertAll(x => x.Name);
            SceneCombo.ItemsSource = scenes;
        }

        private void RefreshSources()
        {
            if (!Command.obs.IsConnected)
            {
                SourceCombo.ItemsSource = null;
                return;
            }

            List<string> sources = new List<string>();

            // Get a list of all sources that have audio
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
                Command.saveToFilePath = dlg.FileName.Replace("\\", "/").Replace(".png", " - {TIMESTAMP}.png");
            }
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
