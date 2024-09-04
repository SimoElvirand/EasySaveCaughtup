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
using WPF.view.components;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = sidebar.SelectedItem as CustomNavigationButton;
            navframe.Navigate(selected.NavUri);
        }
        //public void ShowNotification(string message)
        //{
            // Afficher la notification dans une boîte de message (ou toute autre méthode que vous préférez)
           // MessageBox.Show(message, "Backup Status", MessageBoxButton.OK, MessageBoxImage.Information);
        //}

        public void ShowNotification(string message)
        {
            // Utilisez Dispatcher pour invoquer l'affichage de la fenêtre sur le thread de l'interface utilisateur
            Dispatcher.InvokeAsync(() =>
            {
                // Créez une nouvelle fenêtre pour la notification
                var dialog = new Window
                {
                    Title = "Notification",
                    Width = 300,
                    Height = 150,
                    Content = new TextBlock
                    {
                        Text = message,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                    },
                    WindowStartupLocation = WindowStartupLocation.CenterOwner, // Place la fenêtre au centre de son propriétaire
                    Owner = this // Définit la fenêtre principale comme propriétaire
                };

                // Affiche la fenêtre en tant que boîte de dialogue
                dialog.ShowDialog();
            });
        }

    }
}
