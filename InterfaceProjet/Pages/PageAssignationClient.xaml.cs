using InterfaceClient.Singletons;
using InterfaceProjet.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageAssignationClient : Page
    {
        public PageAssignationClient()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Toujours charger les clients
            var s = SingletonClient.getInstance();
            s.getAllClients();
            lvClients.ItemsSource = s.Liste;
        }

        private void tbRechercheClient_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            string motCle = sender.Text.Trim();
            var s = SingletonClient.getInstance();

            if (string.IsNullOrWhiteSpace(motCle))
                s.getAllClients();
            else
                s.RechercherClients(motCle);

            lvClients.ItemsSource = s.Liste;
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private async void btnChoisir_Click(object sender, RoutedEventArgs e)
        {
            // Client sélectionné
            Client? clientSelectionne = lvClients.SelectedItem as Client;

            if (clientSelectionne == null)
            {
                await AfficherDialogue("Aucun client sélectionné",
                    "Veuillez sélectionner un client dans la liste.");
                return;
            }

            // On retourne à la page d'ajout de projet avec le client choisi
            Frame.Navigate(typeof(PageAjoutPojet), clientSelectionne);
        }

        //  Méthode helper pour afficher un dialogue de façon sécuritaire
        private async System.Threading.Tasks.Task AfficherDialogue(string titre, string contenu)
        {
            await System.Threading.Tasks.Task.Delay(100); // Petit délai de sécurité

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