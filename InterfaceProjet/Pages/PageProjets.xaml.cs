using InterfaceAdmin.Singletons;
using InterfaceProjet.Classes;
using InterfaceProjet.Singletons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageProjets : Page
    {
        private readonly SingletonProjet _projetsSingleton;

        public PageProjets()
        {
            this.InitializeComponent();

            _projetsSingleton = SingletonProjet.getInstance();
            bool estAdmin = SingletonAdmin.getInstance().EstConnecte();

            // Choisir le modèle d'affichage selon le rôle administrateur
            if (estAdmin)
            {
                listeProjets.ItemTemplate = (DataTemplate)this.Resources["ProjetTemplateAdmin"];
            }
            else
            {
                listeProjets.ItemTemplate = (DataTemplate)this.Resources["ProjetTemplateUser"];
                btnAjouter.Visibility = Visibility.Collapsed;
            }

            // Lier la liste à la collection du singleton
            listeProjets.ItemsSource = _projetsSingleton.Liste;
            _projetsSingleton.getAllProjets();
        }

        // Clic sur une carte de projet
        private async void listeProjets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listeProjets.SelectedItem is Projet projet)
            {
                // 1) Récupérer les assignations
                var assignations = SingletonAssignation
                                       .getInstance()
                                       .getAssignationsParProjet(projet.NumeroProjet);

                var listeAssignations = new ObservableCollection<Assignation>(assignations);

                // 2) Ouvrir la boîte de dialogue de détails
                var dialog = new ProjetDetailsDialog(
                    projet,
                    listeAssignations,
                    null
                )
                {
                    XamlRoot = this.Content.XamlRoot
                };

                await dialog.ShowAsync();

                listeProjets.SelectedItem = null;
            }
        }

        // Recherche dynamique
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

        // Bouton "Assigner"
        private async void assigner_Click(object sender, RoutedEventArgs e)
        {
            // 1) Récupérer le projet à partir du DataContext
            if (sender is not FrameworkElement fe || fe.DataContext is not Projet projetSelectionne)
            {
                await new ContentDialog
                {
                    Title = "Erreur",
                    Content = "Impossible de récupérer le projet sélectionné.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                }.ShowAsync();
                return;
            }

            // 2) Bloquer si le projet est déjà terminé
            if (projetSelectionne.EstTermine() || projetSelectionne.Statut == "Terminé")
            {
                await new ContentDialog
                {
                    Title = "Assignation impossible",
                    Content = "Vous ne pouvez pas assigner un employé à un projet terminé.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                }.ShowAsync();
                return;
            }

            // 3) Vérifier le Frame de navigation
            Frame frame = this.Frame;

            if (frame == null)
            {
                await new ContentDialog
                {
                    Title = "Erreur de navigation",
                    Content = "Impossible de trouver le Frame pour naviguer vers la page d'assignation.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                }.ShowAsync();
                return;
            }

            // 4) Navigation vers la page d'assignation
            try
            {
                frame.Navigate(typeof(PageAssignationEmploye), projetSelectionne);
            }
            catch (Exception ex)
            {
                await new ContentDialog
                {
                    Title = "Erreur lors de la navigation",
                    Content = $"Navigation vers la page d'assignation impossible.\n\nDétails : {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                }.ShowAsync();
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

                await dialog.ShowAsync();

                // Si l'utilisateur souhaite réassigner le client
                if (dialog.VeutChangerClient)
                {
                    Frame.Navigate(typeof(PageAssignationClient), projet);
                }
            }
        }

        // Bouton "Terminer"
        private async void Terminer_Click(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            Projet? projet = fe?.DataContext as Projet;

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

            // 1) Mise à jour du statut du projet
            SingletonProjet.getInstance().TerminerProjet(projet.NumeroProjet);

            // 2) Libérer les employés associés
            SingletonAssignation.getInstance().LibererEmployesProjet(projet.NumeroProjet);

            // 3) Recharger la liste
            SingletonProjet.getInstance().getAllProjets();
        }

        // Bouton "Supprimer"
        private async void ButtonSupprimer_Click(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            Projet? projet = element?.DataContext as Projet;

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
            }
        }
    }
}
