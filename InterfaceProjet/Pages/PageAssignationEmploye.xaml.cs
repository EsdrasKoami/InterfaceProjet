using InterfaceEmploye.Singletons;
using InterfaceProjet.Classes;
using InterfaceProjet.Singletons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;

// Alias pour éviter toute confusion si tu as une page qui s'appelle Employe
using EmployeModel = InterfaceProjet.Classes.Employe;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageAssignationEmploye : Page
    {
        private Projet projetCourant;

        public PageAssignationEmploye()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Projet p)
            {
                projetCourant = p;

                // Charger les employés disponibles
                var singleton = SingletonEmploye.getInstance();
                singleton.getEmployesDisponibles();
                lvEmployes.ItemsSource = singleton.Liste;
            }
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void btnChoisirEmploye_Click(object sender, RoutedEventArgs e)
        {
            // Sécurité : projet présent ?
            if (projetCourant == null)
            {
                await ShowMessage("Erreur", "Aucun projet en contexte.");
                return;
            }

            // Récupérer l’employé sélectionné
            var employe = lvEmployes.SelectedItem as EmployeModel;

          
            if (employe == null)
            {
                await ShowMessage("Erreur", "Veuillez sélectionner un employé.");
                return;
            }

            // Vérifier le nombre max d’employés (5)
            if (projetCourant.NbEmployesAssignes >= 5)
            {
                await ShowMessage("Erreur",
                    "Maximum de 5 employés atteint pour ce projet.");
                return;
            }

            // Budget restant (on peut utiliser la méthode du SingletonProjet pour être sûr)
            decimal budgetRestant =
                SingletonProjet.getInstance().GetBudgetRestant(projetCourant.NumeroProjet);

            if (budgetRestant <= 0)
            {
                await ShowMessage("Erreur", "Budget épuisé pour ce projet.");
                return;
            }

            // Demander le nombre d’heures à l’utilisateur
            await DemanderHeures(employe, budgetRestant);
        }

       
        private async Task DemanderHeures(EmployeModel employe, decimal budgetRestant)
        {
            var panel = new StackPanel();

            var info = new TextBlock
            {
                Text = $"Employé : {employe.Prenom} {employe.Nom}\n" +
                       $"Taux : {employe.TauxHoraire:F2} $/h\n" +
                       $"Budget restant : {budgetRestant:C}",
                Margin = new Thickness(0, 0, 0, 10)
            };
            panel.Children.Add(info);

            var tbHeures = new TextBox
            {
                Header = "Nombre d'heures",
                Text = "40"
            };
            panel.Children.Add(tbHeures);

            var dialog = new ContentDialog
            {
                Title = "Nombre d'heures",
                Content = panel,
                PrimaryButtonText = "Assigner",
                CloseButtonText = "Annuler",
                XamlRoot = this.Content.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
                return;

            if (!decimal.TryParse(tbHeures.Text, out var heures) || heures <= 0)
            {
                await ShowMessage("Erreur", "Nombre d'heures invalide.");
                return;
            }

            decimal salaire = employe.TauxHoraire * heures;

            if (salaire > budgetRestant)
            {
                await ShowMessage(
                    "Erreur",
                    $"Budget insuffisant !\nSalaire : {salaire:C}\nBudget restant : {budgetRestant:C}"
                );
                return;
            }

            await Assigner(employe, heures);
        }

        
        private async Task Assigner(EmployeModel employe, decimal heures)
        {
            try
            {
                SingletonAssignation.getInstance().AjouterAssignationEmploye(
                    employe.Matricule,
                    projetCourant.NumeroProjet,
                    heures
                );

            
                projetCourant.NbEmployesAssignes += 1;
                projetCourant.TotalSalaires += employe.TauxHoraire * heures;

                await ShowMessage(
                    "Succès",
                    $"{employe.Prenom} {employe.Nom} a été assigné au projet {projetCourant.NumeroProjet}."
                );

                Frame.GoBack();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                await ShowMessage("Erreur SQL", ex.Message);
            }
            catch (Exception ex)
            {
                await ShowMessage("Erreur", ex.Message);
            }
        }

        
        private void tbRechercheEmploye_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var motCle = sender.Text.Trim();
            var singleton = SingletonEmploye.getInstance();

<<<<<<< Updated upstream
            if (string.IsNullOrWhiteSpace(motCle))
            {
                singleton.getEmployesDisponibles();
            }
            else
            {
                singleton.RechercherEmployesTout(motCle);
            }

            lvEmployes.ItemsSource = singleton.Liste;
        }

        
        private async Task ShowMessage(string title, string content)
        {
            var dlg = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await dlg.ShowAsync();
        }
=======
    private void tbRechercheEmploye_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        var motCle = sender.Text.Trim();
        var singleton = SingletonEmploye.getInstance();

        if (string.IsNullOrWhiteSpace(motCle))
        {
            singleton.getEmployesDisponibles();

        }
        else
        {
            singleton.RechercherEmployesTout(motCle);
        }

        lvEmployes.ItemsSource = singleton.Liste;
>>>>>>> Stashed changes
    }
}
