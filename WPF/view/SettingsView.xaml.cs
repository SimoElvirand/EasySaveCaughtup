using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF.model;
using WPF.view_model;

namespace WPF.view
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Page
    {
        public SettingsView()
        {
            InitializeComponent();
            LanguageManager.Instance.LanguageChanged += LanguageChangedHandler;

            AppSettingsViewModel appSettingsViewModel = new AppSettingsViewModel();
            this.DataContext = appSettingsViewModel;
            LanguageManager.Instance.LanguageChanged += LanguageChangedHandler;
            UpdateUI();

        }
        
        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            // Update UI elements with localized resources here
            // For example, update TextBlock.Text, Button.Content, etc.
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected language ComboBoxItem
            StackPanel selectedLanguageItem = (StackPanel)LanguageComboBox.SelectedItem;

            // Retrieve the Tag property of the selected ComboBoxItem
            string selectedLanguageTag = selectedLanguageItem.Tag.ToString();

            LanguageManager.Instance.SetLanguage(selectedLanguageTag);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
