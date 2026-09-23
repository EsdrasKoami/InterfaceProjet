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

        // Modification d'un client
        private async void ButtonModifier_Click(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var client = fe?.DataContext as Client;
            if (client == null) return;

            var dlg = new ModifierClientDialog(client)
            {
                XamlRoot = this.XamlRoot  
            };
            await dlg.ShowAsync();

            var singleton = SingletonClient.getInstance();
            singleton.getAllClients();
            lvClients.ItemsSource = singleton.Liste;
        }

        // Suppression d'un client
        private async void ButtonSupprimer_Click(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var client = fe?.DataContext as Client;
            if (client == null) return;

            var dlg = new ContentDialog
            {
                Title = "Supprimer le client",
                Content = $"Voulez-vous vraiment supprimer {client.Nom} (ID : {client.IdClient}) ?",
                PrimaryButtonText = "Supprimer",
                CloseButtonText = "Annuler",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot 
            };

            var result = await dlg.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    SingletonClient.getInstance().SupprimerClient(client.IdClient);

                    var singleton = SingletonClient.getInstance();
                    singleton.getAllClients();
                    lvClients.ItemsSource = singleton.Liste;

                    await AfficherDialogue("Succès", $"Le client {client.Nom} a été supprimé avec succès.");
                }
                catch (Exception ex)
                {
                    await AfficherDialogue("Erreur", $"Une erreur est survenue lors de la suppression : {ex.Message}");
                }
            }
        }

        // Recherche dynamique
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

        // Méthode d'affichage sécurisée pour boîte de dialogue
        private async System.Threading.Tasks.Task AfficherDialogue(string titre, string contenu)
        {
            await System.Threading.Tasks.Task.Delay(100);

            var dialog = new ContentDialog
            {
                Title = titre,
                Content = contenu,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }
}
