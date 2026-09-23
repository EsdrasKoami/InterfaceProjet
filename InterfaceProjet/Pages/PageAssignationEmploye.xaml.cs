using InterfaceEmploye.Singletons;
using InterfaceProjet.Classes;
using InterfaceProjet.Singletons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;

using EmployeModel = InterfaceProjet.Classes.Employe;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageAssignationEmploye : Page
    {
        private Projet? projetCourant;

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
                singleton.GetEmployesDisponibles();
                lvEmployes.ItemsSource = singleton.Liste;
            }
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void btnChoisirEmploye_Click(object sender, RoutedEventArgs e)
        {
            // Vérification : projet présent ?
            if (projetCourant == null)
            {
                await AfficherMessage("Erreur", "Aucun projet en contexte.");
                return;
            }

            // Récupérer l'employé sélectionné
            var employe = lvEmployes.SelectedItem as EmployeModel;

            if (employe == null)
            {
                await AfficherMessage("Erreur", "Veuillez sélectionner un employé.");
                return;
            }

            // Recharger le nombre d'employés assignés
            projetCourant.NbEmployesAssignes = SingletonAssignation
                .getInstance()
                .getNombreAssignationsProjet(projetCourant.NumeroProjet);

            // Vérifier le maximum requis
            if (projetCourant.NbEmployesAssignes >= projetCourant.NbEmployesRequis)
            {
                await AfficherMessage("Erreur",
                    $"Maximum de {projetCourant.NbEmployesRequis} employés atteint pour ce projet.");
                return;
            }

            // Vérification du budget restant 
            decimal budgetRestant =
                SingletonProjet.getInstance().GetBudgetRestant(projetCourant.NumeroProjet);

            if (budgetRestant <= 0)
            {
                await AfficherMessage("Erreur", "Budget épuisé pour ce projet.");
                return;
            }

            // Demander le nombre d'heures à l'utilisateur
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
                XamlRoot = this.XamlRoot  
            };

            var result = await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
                return;

            if (!decimal.TryParse(tbHeures.Text, out var heures) || heures <= 0)
            {
                await AfficherMessage("Erreur", "Nombre d'heures invalide.");
                return;
            }

            decimal salaire = employe.TauxHoraire * heures;

            if (salaire > budgetRestant)
            {
                await AfficherMessage(
                    "Erreur",
                    $"Budget insuffisant !\nSalaire : {salaire:C}\nBudget restant : {budgetRestant:C}"
                );
                return;
            }

            await Assigner(employe, heures);
        }

        private async Task Assigner(EmployeModel employe, decimal heures)
        {
            if (projetCourant == null) return;

            try
            {
                SingletonAssignation.getInstance().AjouterAssignationEmploye(
                    employe.Matricule,
                    projetCourant.NumeroProjet,
                    heures
                );

                // Mettre à jour le projet
                projetCourant.NbEmployesAssignes += 1;
                projetCourant.TotalSalaires += employe.TauxHoraire * heures;

                await AfficherMessage(
                    "Succès",
                    $"{employe.Prenom} {employe.Nom} a été assigné au projet {projetCourant.NumeroProjet}."
                );

                Frame.GoBack();
            }
            catch (Exception ex)
            {
                await AfficherMessage("Erreur", ex.Message);
            }
        }

        private void tbRechercheEmploye_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var motCle = sender.Text.Trim();
            var singleton = SingletonEmploye.getInstance();

            if (string.IsNullOrWhiteSpace(motCle))
            {
                singleton.GetEmployesDisponibles();
            }
            else
            {
                singleton.RechercherEmployesTout(motCle);
            }

            lvEmployes.ItemsSource = singleton.Liste;
        }

        // Méthode d'affichage sécurisée pour boîte de dialogue
        private async Task AfficherMessage(string title, string content)
        {
            await System.Threading.Tasks.Task.Delay(100);

            var dlg = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot  
            };

            await dlg.ShowAsync();
        }
    }
}
