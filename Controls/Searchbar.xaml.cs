using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace NOOBS_CMDR.Controls
{
    /// <summary>
    /// Interaction logic for Searchbar.xaml
    /// </summary>
    public partial class Searchbar : UserControl
    {
        #region Properties and Events

        public string Hint
        {
            get { return (string)GetValue(HintProperty); }
            set { SetValue(HintProperty, value); }
        }

        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register("Hint", typeof(string),
              typeof(Searchbar), new PropertyMetadata(null));

        public string Text
        {
            get { return SearchTextBox.Text; }
            set { SearchTextBox.Text = value; }
        }

        public event EventHandler SearchTextChanged;

        #endregion

        public Searchbar()
        {
            InitializeComponent();

            ClearButton.Visibility = Visibility.Collapsed;
            HintLabel.Visibility = Visibility.Visible;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTextChanged != null)
            {
                SearchTextChanged(this, EventArgs.Empty);
            }

            if (SearchTextBox.Text == "")
            {
                if (SearchTextBox.IsFocused == false)
                {
                    ClearButton.Visibility = Visibility.Collapsed;
                    HintLabel.Visibility = Visibility.Visible;
                    return;
                }
            }
            ClearButton.Visibility = Visibility.Visible;
            HintLabel.Visibility = Visibility.Collapsed;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
        }

        private void SearchTextBox_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (SearchTextBox.Text == "")
            {
                if (SearchTextBox.IsFocused == true)
                {
                    ClearButton.Visibility = Visibility.Collapsed;
                    HintLabel.Visibility = Visibility.Visible;
                    return;
                }
            }
            ClearButton.Visibility = Visibility.Visible;
            HintLabel.Visibility = Visibility.Collapsed;
        }

        private void HintLabel_Loaded(object sender, RoutedEventArgs e)
        {
            HintLabel.Content = Hint;
        }
    }
}
