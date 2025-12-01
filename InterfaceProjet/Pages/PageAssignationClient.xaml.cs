using InterfaceClient.Singletons;
using InterfaceProjet.Classes;
using InterfaceProjet.Singletons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Text.RegularExpressions;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageAssignationClient : Page
    {
        private Projet projetCourant;
        public PageAssignationClient()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Projet p)
            {
                projetCourant = p;

                // On charge toujours les clients
                var s = SingletonClient.getInstance();
                s.getAllClients();
                lvClients.ItemsSource = s.Liste;
            }
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
            // Sécurité : projet bien reçu ?
            if (projetCourant == null)
            {
                var dlg = new ContentDialog
                {
                    Title = "Aucun projet",
                    Content = "Aucun projet n'a été reçu pour l'assignation.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dlg.ShowAsync();
                return;
            }

            // Client sélectionné dans la liste
            Client clientSelectionne = lvClients.SelectedItem as Client;

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

            try
            {
                // Appel à ta procédure stockée
                SingletonProjet.getInstance()
                               .AssocierClientAuProjet(projetCourant.NumeroProjet,
                                                       clientSelectionne.IdClient);

                // Mise à jour locale
                projetCourant.IdClient = clientSelectionne.IdClient;
                projetCourant.NomClient = clientSelectionne.Nom;

                // Message de confirmation
                string message = "Le client " + clientSelectionne.Nom +
                                 " a été associé au projet " + projetCourant.NumeroProjet + ".";

                var confirm = new ContentDialog
                {
                    Title = "Assignation réussie",
                    Content = message,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await confirm.ShowAsync();

                // Retour à la liste des projets
                Frame.Navigate(typeof(PageProjets));
            }
            catch (Exception ex)
            {
                var dlgErr = new ContentDialog
                {
                    Title = "Erreur d'assignation",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dlgErr.ShowAsync();
            }
        }

    }
}
