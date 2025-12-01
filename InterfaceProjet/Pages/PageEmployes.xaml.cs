
using InterfaceProjet.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using InterfaceEmploye.Singletons;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageEmployes : Page
    {
        public PageEmployes()
        {
            InitializeComponent();

            var singleton = SingletonEmploye.getInstance();
            lvEmployes.ItemsSource = singleton.Liste;
            singleton.getEmployesDisponibles();   // charge la liste
        }

        // SUPPRIMER
        private async void supprimer_Click(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var emp = fe?.DataContext as InterfaceProjet.Classes.Employe;
            if (emp == null) return;

            var dlg = new ContentDialog
            {
                Title = "Supprimer l'employé",
                Content = $"Voulez-vous vraiment supprimer {emp.Prenom} {emp.Nom} ({emp.Matricule}) ?",
                PrimaryButtonText = "Supprimer",
                CloseButtonText = "Annuler",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.Content.XamlRoot
            };

            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // appel au singleton pour supprimer
                SingletonEmploye.getInstance().SupprimerEmploye(emp.Matricule);

                // recharger la liste
                var singleton = SingletonEmploye.getInstance();
                singleton.getEmployesDisponibles();
                lvEmployes.ItemsSource = singleton.Liste;
            }
        }

        // MODIFIER
        private async void modifier_Click(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var emp = fe?.DataContext as Employe;
            if (emp == null) return;

            var dlg = new ModifierEmployeDialog(emp)
            {
                XamlRoot = this.Content.XamlRoot
            };

            await dlg.ShowAsync();
           
        }

        // RECHERCHE
        private void tbRechercheEmploye_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var motCle = sender.Text.Trim();
            var singleton = SingletonEmploye.getInstance();

            if (string.IsNullOrWhiteSpace(motCle))
                singleton.getEmployesDisponibles();
            else
                singleton.RechercherEmployesTout(motCle);

            lvEmployes.ItemsSource = singleton.Liste;
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageAjoutEmploye));
        }
    }
}
