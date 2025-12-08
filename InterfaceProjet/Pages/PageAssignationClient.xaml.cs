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
                var dlg = new ContentDialog
                {
                    Title = "Aucun client sélectionné",
                    Content = "Veuillez sélectionner un client dans la liste.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dlg.ShowAsync();
                return;
            }

            // On retourne à la page d’ajout de projet avec le client choisi
            Frame.Navigate(typeof(PageAjoutPojet), clientSelectionne);
        }
    }
}
