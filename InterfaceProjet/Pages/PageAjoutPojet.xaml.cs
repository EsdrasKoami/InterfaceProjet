using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using InterfaceProjet.Singletons;
using InterfaceProjet.Classes;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageAjoutPojet : Page
    {
        private Client clientSelectionne;

        //  Variables pour sauvegarder l'état du formulaire
        private string titreTemp = "";
        private DateTime dateDebutTemp = DateTime.Now;
        private string descriptionTemp = "";
        private double budgetTemp = 0;
        private int nbEmployesTemp = 1;
        private double totalSalairesTemp = 0;

        public PageAjoutPojet()
        {
            InitializeComponent();

            // Activer la mise en cache de la page pour conserver l'état du formulaire
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Si on revient de PageAssignationClient avec un client sélectionné
            if (e.Parameter is Client client)
            {
                clientSelectionne = client;
                idClient.Text = client.IdClient.ToString();

                //  Restaurer les valeurs sauvegardées
                RestaurerFormulaire();
            }
        }

        // Méthode pour sauvegarder l'état du formulaire
        private void SauvegarderFormulaire()
        {
            titreTemp = tbxtitre.Text.Trim();
            dateDebutTemp = dpDateDebut.Date.DateTime;
            descriptionTemp = tbxDescription.Text.Trim();
            budgetTemp = nbxBudget.Value;
            nbEmployesTemp = (int)nbrEmploye.Value;
            totalSalairesTemp = totalSalaire.Value;
        }

        //  Méthode pour restaurer l'état du formulaire
        private void RestaurerFormulaire()
        {
            tbxtitre.Text = titreTemp;
            dpDateDebut.Date = dateDebutTemp;
            tbxDescription.Text = descriptionTemp;
            nbxBudget.Value = budgetTemp;
            nbrEmploye.Value = nbEmployesTemp;
            totalSalaire.Value = totalSalairesTemp;
        }

        private void ButtonAnnuler_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null && this.Frame.CanGoBack)
                this.Frame.GoBack();
        }

        private async void ButtonEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Réinitialiser messages d'erreur
            tblErrTitre.Text = "";
            tblErrDateDebut.Text = "";
            tblErrDescription.Text = "";
            tblErrBudget.Text = "";
            tblErrNbrEmploye.Text = "";
            tblErrTotalsalaire.Text = "";
            tblErrIdClient.Text = "";

            bool valide = true;

            string titre = tbxtitre.Text.Trim();
            string description = tbxDescription.Text.Trim();

            // Titre
            if (string.IsNullOrWhiteSpace(titre))
            {
                tblErrTitre.Text = "Le titre est obligatoire.";
                valide = false;
            }

            // Date début
            DateTime dateDebut = dpDateDebut.Date.DateTime;
            if (dateDebut > DateTime.Now.Date.AddYears(1))
            {
                tblErrDateDebut.Text = "Date de début invalide.";
                valide = false;
            }

            // Description
            if (string.IsNullOrWhiteSpace(description))
            {
                tblErrDescription.Text = "La description est obligatoire.";
                valide = false;
            }

            // Budget
            double budgetDouble = nbxBudget.Value;
            if (double.IsNaN(budgetDouble) || budgetDouble <= 0)
            {
                tblErrBudget.Text = "Entrez un budget positif.";
                valide = false;
            }

            // Nombre d'employés
            int nbEmployes = (int)nbrEmploye.Value;
            if (nbEmployes <= 0 || nbEmployes > 5)
            {
                tblErrNbrEmploye.Text = "Entrez un nombre entre 1 et 5.";
                valide = false;
            }

            // Total des salaires
            double totalSalairesDouble = totalSalaire.Value;
            if (double.IsNaN(totalSalairesDouble) || totalSalairesDouble < 0)
            {
                tblErrTotalsalaire.Text = "Le total des salaires ne peut pas être négatif.";
                valide = false;
            }

            // Client
            if (clientSelectionne == null)
            {
                tblErrIdClient.Text = "Vous devez choisir un client.";
                valide = false;
            }

            if (!valide)
            {
                await AfficherDialogue("Formulaire invalide",
                    "Veuillez corriger les erreurs indiquées en rouge.");
                return;
            }

            decimal budget = (decimal)budgetDouble;
            decimal totalSalaires = (decimal)totalSalairesDouble;

            try
            {
                //  Plus besoin de générer le numéro, le trigger s'en occupe
                SingletonProjet
                    .getInstance()
                    .ajouterProjetAvecProcedure(
                        titre,
                        dateDebut,
                        description,
                        budget,
                        nbEmployes,
                        clientSelectionne.IdClient
                    );

                await AfficherDialogue("Succès", "Le projet a été ajouté avec succès.");
                Frame.Navigate(typeof(PageProjets));
            }
            catch (Exception ex)
            {
                await AfficherDialogue("Erreur",
                    $"Une erreur est survenue lors de l'ajout du projet : {ex.Message}");
            }
        }

        private void ButtonAssigner_Click(object sender, RoutedEventArgs e)
        {
            //  Sauvegarder le formulaire avant de naviguer , les donnees disparaissaient quand on partait sur la page assignation pour assigner le client au projet 
            SauvegarderFormulaire();

            Frame.Navigate(typeof(PageAssignationClient));
        }

        //  Méthode helper pour afficher un dialogue de façon sécuritaire
        private async System.Threading.Tasks.Task AfficherDialogue(string titre, string contenu)
        {
            await System.Threading.Tasks.Task.Delay(100);

            var dialog = new ContentDialog
            {
                Title = titre,
                Content = contenu,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot 
            };

            await dialog.ShowAsync();
        }
    }
}