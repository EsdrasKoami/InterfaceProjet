using InterfaceClient.Singletons  ;
using InterfaceProjet.Classes;
using InterfaceProjet.Singletons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace InterfaceProjet.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
                
            }
        }
        private  void tbRechercheClient_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            string motCle = sender.Text.Trim();
            if (string.IsNullOrWhiteSpace(motCle))
            {
                // Si la recherche est vide on recharge tous les clIents
              SingletonClient.getInstance().getAllClients();
            }
            else
            {
                // Filtrage
                SingletonClient.getInstance().RechercherClients(motCle);
            }
            lvClients.ItemsSource = SingletonClient.getInstance().Liste;
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnChoisir_Click(object sender, RoutedEventArgs e)
        {
            // Sécurité : vérifier qu'on a bien un projet courant
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

            // 1) Vérifier qu’un client est sélectionné
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

            // 2) Vérifier si le projet a déjà un client associé
            //    (si IdClient != 0, on considère qu'un client est déjà lié)
            if (projetCourant.IdClient != 0)
            {
                var dlgExiste = new ContentDialog
                {
                    Title = "Assignation impossible",
                    Content = $"Ce projet a déjà un client associé (ID {projetCourant.IdClient}).\n" +
                              $"Vous ne pouvez pas lui assigner un autre client.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await dlgExiste.ShowAsync();
                return;
            }

            // 3) On tente l’assignation
            try
            {
                // Appel de ta procédure via le singleton
                SingletonAssignation
                    .getInstance()
                    .AssocierClientAuProjet(projetCourant.NumeroProjet, clientSelectionne.IdClient);

                // Mettre aussi à jour l'objet en mémoire, pour que l’UI reste cohérente
                projetCourant.IdClient = clientSelectionne.IdClient;
                projetCourant.NomClient = clientSelectionne.Nom;

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
                // Si la procédure lève un SIGNAL (erreur SQL), on l'affiche dans un dialog
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

