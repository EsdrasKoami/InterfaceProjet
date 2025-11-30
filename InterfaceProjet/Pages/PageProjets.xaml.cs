using InterfaceProjet.Classes;
using InterfaceProjet.Singletons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageProjets : Page
    {
        private readonly SingletonProjet _projetsSingleton;
        private bool _isDialogOpen = false; // ?? verrou pour ContentDialog

        public PageProjets()
        {
            this.InitializeComponent();

            _projetsSingleton = SingletonProjet.getInstance();

            // Lier la GridView à la liste des projets du singleton
            listeProjets.ItemsSource = _projetsSingleton.Liste;
            _projetsSingleton.getAllProjets();
        }

        /// <summary>
        /// Helper pour s'assurer qu'un seul ContentDialog est ouvert à la fois.
        /// </summary>
        private async System.Threading.Tasks.Task<ContentDialogResult> ShowSingleDialogAsync(ContentDialog dialog)
        {
            if (_isDialogOpen)
            {
                // Un autre dialog est déjà ouvert ? on ne fait rien
                return ContentDialogResult.None;
            }

            _isDialogOpen = true;

            try
            {
                return await dialog.ShowAsync();
            }
            finally
            {
                _isDialogOpen = false;
            }
        }

        // Quand on clique sur une carte de projet dans la GridView
        private async void listeProjets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listeProjets.SelectedItem is Projet projet)
            {
                var dialog = new ProjetDetailsDialog(
                    projet,
                    new ObservableCollection<Assignation>(),
                    null
                )
                {
                    XamlRoot = this.Content.XamlRoot
                };

                await ShowSingleDialogAsync(dialog);

                // On désélectionne l'item après fermeture du dialog
                listeProjets.SelectedItem = null;
            }
        }

        // Recherche de projets (tu pourras compléter la logique plus tard)
        private void tbRechercheProjet_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Exemple de logique (à adapter si tu veux l’AutoSuggestBox)
            // string motCle = (sender as TextBox)?.Text.Trim() ?? "";
            // _projetsSingleton.rechercherProjets(motCle);
            // listeProjets.ItemsSource = _projetsSingleton.Liste;
        }

        // Bouton "Ajouter un projet"
        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            // Ici on ne dépend pas de la sélection
            // On navigue simplement vers la page d’ajout de projet
            Frame.Navigate(typeof(PageAjoutPojet));
        }

        // AutoSuggestBox pour recherche (si tu veux l’utiliser au lieu de TextBox)
        private void tbRechercheProjet_TextChanged_1(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Exemple :
            // string motCle = sender.Text.Trim();
            // _projetsSingleton.rechercherProjets(motCle);
            // listeProjets.ItemsSource = _projetsSingleton.Liste;
        }

        // Bouton "Assigner" dans chaque carte de projet
        private async void assigner_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer le projet à partir du DataContext du bouton
            var recupere = sender as FrameworkElement;
            Projet projetSelectionne = recupere?.DataContext as Projet;

            if (projetSelectionne == null)
            {
                var dlgErreur = new ContentDialog
                {
                    Title = "Erreur",
                    Content = "Impossible de récupérer le projet sélectionné.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await ShowSingleDialogAsync(dlgErreur);
                return;
            }

            var dialogChoix = new ContentDialog
            {
                Title = $"Projet {projetSelectionne.NumeroProjet}",
                Content = "Que souhaitez-vous faire ?",
                PrimaryButtonText = "Assigner client",
                SecondaryButtonText = "Assigner employé",
                CloseButtonText = "Annuler",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.Content.XamlRoot
            };

            var result = await ShowSingleDialogAsync(dialogChoix);

            switch (result)
            {
                case ContentDialogResult.Primary:
                    // Assigner client
                    if (Frame != null)
                    {
                        Frame.Navigate(typeof(PageAssignationClient), projetSelectionne);
                    }
                    else
                    {
                        Debug.WriteLine("Frame est null, navigation vers PageAssignationClient impossible.");
                    }
                    break;

                case ContentDialogResult.Secondary:
                    // Assigner employé
                    if (Frame != null)
                    {
                        Frame.Navigate(typeof(PageAssignationEmploye), projetSelectionne);
                    }
                    else
                    {
                        Debug.WriteLine("Frame est null, navigation vers PageAssignationEmploye impossible.");
                    }
                    break;

                case ContentDialogResult.None:
                default:
                    // Annuler / rien faire
                    break;
            }
        }

        // Bouton crayon "Modifier" dans la carte de projet
        private async void ButtonModifier_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is Projet projet)
            {
                var dialog = new ModifierProjetDialog(projet)
                {
                    XamlRoot = this.Content.XamlRoot
                };

                await ShowSingleDialogAsync(dialog);
                // La GridView se mettra à jour automatiquement si Liste est ObservableCollection
            }
        }
    }
}
