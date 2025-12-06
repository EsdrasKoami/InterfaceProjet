using InterfaceAdmin.Singletons;
using InterfaceEmploye.Singletons;
using InterfaceProjet.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageEmployes : Page
    {
        public PageEmployes()
        {
            InitializeComponent();

            var singleton = SingletonEmploye.getInstance();
            bool estAdmin = SingletonAdmin.getInstance().EstConnecte();

            //  CHOISIR LE BON TEMPLATE selon si Admin ou non
            if (estAdmin)
            {
                lvEmployes.ItemTemplate = (DataTemplate)this.Resources["EmployeTemplateAdmin"];
            }
            else
            {
                lvEmployes.ItemTemplate = (DataTemplate)this.Resources["EmployeTemplateUser"];
                btnAjouter.Visibility = Visibility.Collapsed;
            }

          
            lvEmployes.ItemsSource = singleton.Liste;
            singleton.getEmployesDisponibles();
        }

        private async void supprimer_Click(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var emp = fe?.DataContext as Employe;
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
                SingletonEmploye.getInstance().SupprimerEmploye(emp.Matricule);
                var singleton = SingletonEmploye.getInstance();
                singleton.getEmployesDisponibles();
                lvEmployes.ItemsSource = singleton.Liste;
            }
        }

  
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