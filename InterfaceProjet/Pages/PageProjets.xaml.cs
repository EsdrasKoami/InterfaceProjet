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

        private async void assigner_Click(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            Projet projetSelectionne = fe?.DataContext as Projet;

            if (projetSelectionne == null)
                return;

            if (projetSelectionne.Statut == "Terminé")
            {
                var dlg = new ContentDialog
                {
                    Title = "Assignation impossible",
                    Content = "Vous ne pouvez pas assigner un employé à un projet terminé.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dlg.ShowAsync();
                return;
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
       
private async void Terminer_Click(object sender, RoutedEventArgs e)
    {
        var fe = sender as FrameworkElement;
        Projet projet = fe?.DataContext as Projet;

        if (projet == null)
            return;

        if (projet.EstTermine())
            {
            var deja = new ContentDialog
            {
                Title = "Projet déjà terminé",
                Content = $"Le projet {projet.NumeroProjet} est déjà terminé.",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await deja.ShowAsync();
            return;
        }

        var confirm = new ContentDialog
        {
            Title = "Terminer le projet",
            Content = $"Voulez-vous vraiment marquer le projet {projet.NumeroProjet} comme terminé ?\n" +
                      "Les employés assignés seront libérés.",
            PrimaryButtonText = "Terminer",
            CloseButtonText = "Annuler",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.Content.XamlRoot
        };

        var result = await confirm.ShowAsync();
        if (result != ContentDialogResult.Primary)
            return;

        // 1) Appel à ta procédure stockée
        SingletonProjet.getInstance().TerminerProjet(projet.NumeroProjet);

        // 2) Libérer les employés de ce projet
        SingletonAssignation.getInstance().LibererEmployesProjet(projet.NumeroProjet);

        // 3) Recharger la liste (moi je conseille getAllProjets pour garder les terminés visibles)
        SingletonProjet.getInstance().getAllProjets();
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
