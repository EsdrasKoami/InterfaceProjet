using InterfaceProjet.Classes;
using InterfaceProjet.Singletons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageProjets : Page
    {
        private readonly SingletonProjet _projetsSingleton;

        public PageProjets()
        {
            this.InitializeComponent();

            _projetsSingleton = SingletonProjet.getInstance();

            // Lier la GridView à la liste des projets du singleton
            listeProjets.ItemsSource = _projetsSingleton.Liste;
            _projetsSingleton.getAllProjets();
        }

        // Quand on clique sur une carte de projet
        private async void listeProjets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listeProjets.SelectedItem is Projet projet)
            {
                // 1) récupérer les assignations en BD
                var assignations = SingletonAssignation
                                       .getInstance()
                                       .getAssignationsParProjet(projet.NumeroProjet);

                var listeAssignations = new ObservableCollection<Assignation>(assignations);

                // 2) ouvrir le dialog
                var dialog = new ProjetDetailsDialog(
                    projet,
                    listeAssignations,
                    null      // si tu n'utilises pas le dernier paramètre, mets null
                )
                {
                    XamlRoot = this.Content.XamlRoot
                };

                await dialog.ShowAsync();

                listeProjets.SelectedItem = null;
            }
        }



        private void tbRechercheProjet_TextChanged_1(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            string motCle = sender.Text.Trim();

            if (string.IsNullOrWhiteSpace(motCle))
                _projetsSingleton.getAllProjets();
            else
                _projetsSingleton.rechercherProjets(motCle);

            listeProjets.ItemsSource = _projetsSingleton.Liste;
        }

        // Bouton "Ajouter un projet"
        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageAjoutPojet));
        }

        // Bouton "Assigner" (désormais seulement assignation employé)
        private void assigner_Click(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            Projet projetSelectionne = element?.DataContext as Projet;

            if (projetSelectionne == null)
            {
                _ = new ContentDialog
                {
                    Title = "Erreur",
                    Content = "Impossible de récupérer le projet sélectionné.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                }.ShowAsync();
                return;
            }

            if (Frame != null)
            {
                Frame.Navigate(typeof(PageAssignationEmploye), projetSelectionne);
            }
        }

        // Bouton "Modifier"
        private async void ButtonModifier_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is Projet projet)
            {
                var dialog = new ModifierProjetDialog(projet)
                {
                    XamlRoot = this.Content.XamlRoot
                };

                // On attend que la boîte de dialogue se ferme
                await dialog.ShowAsync();

                // Si dans le dialog l’utilisateur a cliqué sur "Changer client"
                if (dialog.VeutChangerClient)
                {
                    // Ici la navigation fonctionne, car on est dans une Page
                    Frame.Navigate(typeof(PageAssignationClient), projet);
                }
            }
        }

        // Bouton "Supprimer"
        private async void ButtonSupprimer_Click(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            Projet projet = element?.DataContext as Projet;

            if (projet == null)
                return;

            var confirm = new ContentDialog
            {
                Title = "Confirmation",
                Content = $"Voulez-vous vraiment supprimer le projet {projet.NumeroProjet} ?",
                PrimaryButtonText = "Supprimer",
                CloseButtonText = "Annuler",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.Content.XamlRoot
            };

            var result = await confirm.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                _projetsSingleton.supprimerProjet(projet.NumeroProjet);
                // Ta liste est rechargée dans supprimerProjet()
            }
        }
    }
}
