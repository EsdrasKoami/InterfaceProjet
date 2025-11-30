using InterfaceClient.Singletons;
using InterfaceProjet.Classes;
using InterfaceProjet.Singletons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageAssignationClient : Page
    {
        Projet projetCourant;

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

                //  Charger les clients disponibles
                SingletonClient.getInstance().getAllClients();
                lvClients.ItemsSource = SingletonClient.getInstance().Liste;
            }
        }

        private void tbRechercheClient_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            string motCle = sender.Text.Trim();

            if (string.IsNullOrWhiteSpace(motCle))
            {
                SingletonClient.getInstance().getAllClients();
            }
            else
            {
                SingletonClient.getInstance().RechercherClients(motCle);
            }

            lvClients.ItemsSource = SingletonClient.getInstance().Liste;
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            // Retour à la page précédente
            Frame.GoBack();
        }

        private async void btnChoisir_Click(object sender, RoutedEventArgs e)
        {
            // ? Sécurité 1: Vérifier qu'on a bien un projet courant
            if (projetCourant == null)
            {
                var dlgProjet = new ContentDialog
                {
                    Title = "Aucun projet en contexte",
                    Content = "Aucun projet n'a été reçu pour l'assignation.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dlgProjet.ShowAsync();
                return;
            }

            //  Sécurité 2: Vérifier qu'un client est sélectionné
            Client clientSelectionne = lvClients.SelectedItem as Client;

            if (clientSelectionne == null)
            {
                var dlg = new ContentDialog
                {
                    Title = "Aucun client sélectionné",
                    Content = "Veuillez d'abord sélectionner un client dans la liste.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dlg.ShowAsync();
                return;
            }

            //  Vérifier si le projet a déjà un client
            if (projetCourant.AUnClient())
            {
                var dlgExiste = new ContentDialog
                {
                    Title = "Assignation impossible",
                    Content = $"Ce projet a déjà un client associé ({projetCourant.NomClient}).\n" +
                              $"Vous ne pouvez pas lui assigner un autre client.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dlgExiste.ShowAsync();
                return;
            }

            //  Tentative d'assignation
            try
            {
                // Appel à la procédure stockée
                SingletonProjet
                    .getInstance()
                    .AssocierClientAuProjet(projetCourant.NumeroProjet, clientSelectionne.IdClient);

                // Mise à jour locale de l'objet
                projetCourant.IdClient = clientSelectionne.IdClient;
                projetCourant.NomClient = clientSelectionne.Nom;

                // Message de confirmation
                var confirm = new ContentDialog
                {
                    Title = "Assignation réussie",
                    Content = $"Le client {clientSelectionne.Nom} a été associé au projet {projetCourant.NumeroProjet}.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await confirm.ShowAsync();

                // Retour à la liste des projets
                Frame.Navigate(typeof(PageProjets));
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                // Erreur SQL (trigger ou procédure)
                var dlgErr = new ContentDialog
                {
                    Title = "Erreur d'assignation",
                    Content = $"Impossible d'associer le client au projet.\n\nDétail : {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dlgErr.ShowAsync();
            }
            catch (Exception ex)
            {
                // Autres erreurs
                var dlgErr = new ContentDialog
                {
                    Title = "Erreur inattendue",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dlgErr.ShowAsync();
            }
        }
    }
}