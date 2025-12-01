using InterfaceClient.Singletons;
using InterfaceProjet.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using MySql.Data.MySqlClient;   // si tu utilises MySql

namespace InterfaceProjet.Pages
{
    public sealed partial class PageClients : Page
    {
        public PageClients()
        {
            InitializeComponent();

            // Charger la liste au démarrage
            var singleton = SingletonClient.getInstance();
            lvClients.ItemsSource = singleton.Liste;
            singleton.getAllClients();
        }

        // Recherche avec AutoSuggestBox
        private void tbRechercheClient_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            string motCle = sender.Text.Trim();
            var singleton = SingletonClient.getInstance();

            if (string.IsNullOrWhiteSpace(motCle))
                singleton.getAllClients();
            else
                singleton.RechercherClients(motCle);

            lvClients.ItemsSource = singleton.Liste;
        }

        // Bouton "Ajouter"
        private void btnAjouterClient_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageAjoutClient));
        }

        // Bouton "Modifier" dans chaque ligne
        private async void ButtonModifier_Click(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            Client client = fe?.DataContext as Client;

            if (client == null)
                return;

            var dlg = new ModifierClientDialog(client)
            {
                XamlRoot = this.Content.XamlRoot
            };

            await dlg.ShowAsync();
        }


        // Bouton "Supprimer" dans chaque ligne
        private async void ButtonSupprimer_Click(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            Client client = fe?.DataContext as Client;

            if (client == null)
                return;

            // Demande de confirmation
            var confirm = new ContentDialog
            {
                Title = "Confirmation",
                Content = $"Voulez-vous vraiment supprimer le client « {client.Nom} » (ID {client.IdClient}) ?",
                PrimaryButtonText = "Supprimer",
                CloseButtonText = "Annuler",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.Content.XamlRoot
            };

            var result = await confirm.ShowAsync();

            if (result != ContentDialogResult.Primary)
                return;

            try
            {
                // Appel au singleton pour supprimer
                SingletonClient.getInstance().SupprimerClient(client.IdClient);

                // Si la suppression réussit, la liste doit être rechargée dans SupprimerClient()
            }
            catch (MySqlException ex)
            {
                // Si le client est lié à un projet (clé étrangère), MySQL va lever une exception
                var dlg = new ContentDialog
                {
                    Title = "Suppression impossible",
                    Content = "Ce client est associé à un ou plusieurs projets. " +
                              "Vous ne pouvez pas le supprimer.\n\n" +
                              $"Détails techniques : {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dlg.ShowAsync();
            }
            catch (Exception ex)
            {
                var dlg = new ContentDialog
                {
                    Title = "Erreur",
                    Content = "Une erreur est survenue lors de la suppression du client : " + ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dlg.ShowAsync();
            }
        }
    }
}
