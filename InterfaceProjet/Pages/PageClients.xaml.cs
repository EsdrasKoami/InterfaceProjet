using InterfaceAdmin.Singletons;
using InterfaceClient.Singletons;
using InterfaceProjet.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageClients : Page
    {
        public PageClients()
        {
            InitializeComponent();

            var singleton = SingletonClient.getInstance();
            bool estAdmin = SingletonAdmin.getInstance().EstConnecte();

            
            if (estAdmin)
            {
                lvClients.ItemTemplate = (DataTemplate)this.Resources["ClientTemplateAdmin"];
            }
            else
            {
                lvClients.ItemTemplate = (DataTemplate)this.Resources["ClientTemplateUser"];
                btnAjouterClient.Visibility = Visibility.Collapsed;
            }

            lvClients.ItemsSource = singleton.Liste;
            singleton.getAllClients(); 
        }

        // MODIFIER
        private async void ButtonModifier_Click(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var client = fe?.DataContext as Client;
            if (client == null) return;

            
            var dlg = new ModifierClientDialog(client)
            {
                XamlRoot = this.Content.XamlRoot
            };
            await dlg.ShowAsync();

           
            var singleton = SingletonClient.getInstance();
            singleton.getAllClients();
            lvClients.ItemsSource = singleton.Liste;
        }

        // SUPPRIMER
        private async void ButtonSupprimer_Click(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var client = fe?.DataContext as Client;
            if (client == null) return;

            var dlg = new ContentDialog
            {
                Title = "Supprimer le client",
                Content = $"Voulez-vous vraiment supprimer {client.Nom} (ID: {client.IdClient}) ?",
                PrimaryButtonText = "Supprimer",
                CloseButtonText = "Annuler",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.Content.XamlRoot
            };

            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                SingletonClient.getInstance().SupprimerClient(client.IdClient);
                var singleton = SingletonClient.getInstance();
                singleton.getAllClients();
                lvClients.ItemsSource = singleton.Liste;
            }
        }

        // RECHERCHE
        private void tbRechercheClient_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var motCle = sender.Text.Trim();
            var singleton = SingletonClient.getInstance();

            if (string.IsNullOrWhiteSpace(motCle))
                singleton.getAllClients();
            else
                singleton.RechercherClients(motCle); 

            lvClients.ItemsSource = singleton.Liste;
        }

        private void btnAjouterClient_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageAjoutClient));
        }
    }
}