using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AutoCompleteComboBox : UserControl
    {
        #region Events

        public event EventHandler TextChanged;
        public event EventHandler TextBox_Clicked;

        #endregion Events


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompleteComboBox"/> class.
        /// </summary>
        public AutoCompleteComboBox()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        ///// <summary>
        ///// Gets or sets the items source.
        ///// </summary>
        ///// <value>The items source.</value>
        //public string Text
        //{
        //    get { return (string)GetValue(TextProperty); }
        //    set { SetValue(TextProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ItemsSource.  
        //// This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty TextProperty =
        //    DependencyProperty.Register("Text"
        //                        , typeof(string)
        //                        , typeof(AutoCompleteComboBox)
        //                        , new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public IEnumerable<string> ItemsSource
        {
            get { return (IEnumerable<string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource"
                                , typeof(IEnumerable<string>)
                                , typeof(AutoCompleteComboBox)
                                , new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the selected value.
        /// </summary>
        /// <value>The selected value.</value>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedValue.  
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text"
                            , typeof(string)
                            , typeof(AutoCompleteComboBox)
                            , new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the selected value.
        /// </summary>
        /// <value>The selected value.</value>
        public string Hint
        {
            get { return (string)GetValue(HintProperty); }
            set { SetValue(HintProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedValue.  
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register("Hint"
                            , typeof(string)
                            , typeof(AutoCompleteComboBox)
                            , new UIPropertyMetadata(string.Empty));

        #endregion

        #region Methods
        /// <summary>
        /// Handles the TextChanged event of the autoTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        void autoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = autoTextBox.Text;
            if (TextChanged != null)
                TextChanged(sender, e);

            if (ItemsSource == null)
                return;

            // Use Linq to Query ItemsSource for resultdata
            string condition = string.Format("{0}%", autoTextBox.Text);
            IEnumerable<string> results = ItemsSource.Where(
                delegate (string s) { return s.ToLower().Contains(autoTextBox.Text.ToLower()) || autoTextBox.Text == ""; });

            if (results.Count() > 0)
            {
                suggestionListBox.ItemsSource = results;
                suggestionListBox.Visibility = Visibility.Visible;
            }
            else
            {
                HideSuggestionListBox();
                suggestionListBox.ItemsSource = null;
            }
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the autoTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        void autoTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
       {
            if (e.Key == Key.Down)
            {
                if (suggestionListBox.SelectedIndex < suggestionListBox.Items.Count)
                {
                    suggestionListBox.SelectedIndex = suggestionListBox.SelectedIndex + 1;
                    suggestionListBox.ScrollIntoView(suggestionListBox.SelectedItem);
                }
            }
            if (e.Key == Key.Up)
            {
                if (suggestionListBox.SelectedIndex > -1)
                {
                    suggestionListBox.SelectedIndex = suggestionListBox.SelectedIndex - 1;
                    suggestionListBox.ScrollIntoView(suggestionListBox.SelectedItem);
                }
            }
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                // Commit the selection
                HideSuggestionListBox();
                e.Handled = (e.Key == Key.Enter);
            }

            if (e.Key == Key.Escape)
            {
                // Cancel the selection
                suggestionListBox.ItemsSource = null;
                HideSuggestionListBox();
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the suggestionListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void suggestionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (suggestionListBox.ItemsSource != null)
            {
                autoTextBox.TextChanged -= new TextChangedEventHandler(autoTextBox_TextChanged);
                if (suggestionListBox.SelectedIndex != -1)
                {
                    autoTextBox.Text = suggestionListBox.SelectedItem.ToString();
                    Text = autoTextBox.Text;
                    if (TextChanged != null)
                        TextChanged(sender, e);
                }
                autoTextBox.TextChanged += new TextChangedEventHandler(autoTextBox_TextChanged);
            }
        }

        #endregion

        private void autoTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            suggestionListBox.ItemsSource = ItemsSource;
            suggestionListBox.Visibility = Visibility.Visible;

            if (TextBox_Clicked != null)
                TextBox_Clicked(sender, e);
        }

        public void HideSuggestionListBox()
        {
            suggestionListBox.Visibility = Visibility.Collapsed;
        }

        private void suggestionListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            HideSuggestionListBox();
        }
    }
}
