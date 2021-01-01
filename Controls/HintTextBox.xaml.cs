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
    /// Interaction logic for HintTextBox.xaml
    /// </summary>
    public partial class HintTextBox : UserControl
    {
        #region Properties and Events

        public string Hint
        {
            get { return (string)GetValue(HintProperty); }
            set { SetValue(HintProperty, value); }
        }

        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register("Hint", typeof(string),
              typeof(HintTextBox), new PropertyMetadata(null));

        public string Text
        {
            get { return TextBox.Text; }
            set { TextBox.Text = value; }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
              typeof(HintTextBox), new PropertyMetadata(null));

        public event TextChangedEventHandler TextChanged;

        #endregion

        public HintTextBox()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextChanged != null)
            {
                TextChanged(this, e);
            }

            if (TextBox.Text == "")
            {
                if (TextBox.IsFocused == false)
                {
                    HintLabel.Visibility = Visibility.Visible;
                    return;
                }
            }
            HintLabel.Visibility = Visibility.Collapsed;
        }

        private void SearchTextBox_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (TextBox.Text == "")
            {
                if (TextBox.IsFocused == true)
                {
                    HintLabel.Visibility = Visibility.Visible;
                    return;
                }
            }
            HintLabel.Visibility = Visibility.Collapsed;
        }
    }
}
