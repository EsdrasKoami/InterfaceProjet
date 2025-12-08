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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace InterfaceProjet.Pages
{
    public sealed partial class PageAjoutPojet : Page
    {
        private Client clientSelectionne;
        public PageAjoutPojet()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Si on revient de PageAssignationClient avec un client sélectionné
            if (e.Parameter is Client client)
            {
                clientSelectionne = client;
                idClient.Text = client.IdClient.ToString();
            }
        }


        private void ButtonAnnuler_Click(object sender, RoutedEventArgs e)
        {
            // Retour à la page précédente si possible
            if (this.Frame != null && this.Frame.CanGoBack)
                this.Frame.GoBack();
        }

        private async void ButtonEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Réinitialiser messages d’erreur
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
                var dlg = new ContentDialog
                {
                    Title = "Formulaire invalide",
                    Content = "Veuillez corriger les erreurs indiquées en rouge.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dlg.ShowAsync();
                return;
            }
            decimal budget = (decimal)budgetDouble;
            decimal totalSalaires = (decimal)totalSalairesDouble;

            try
            {
                // Génération d’un numéro de projet simple (exemple)
                // IdClient-01-Année
                string numeroProjet = $"{clientSelectionne.IdClient}-01-{dateDebut.Year}";

                SingletonProjet
                    .getInstance()
                    .ajouterProjetAvecProcedure(
                        numeroProjet,
                        titre,
                        dateDebut,
                        description,
                        budget,
                        nbEmployes,
                        clientSelectionne.IdClient
                    );

                var dlgOK = new ContentDialog
                {
                    Title = "Succès",
                    Content = "Le projet a été ajouté avec succès.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dlgOK.ShowAsync();

                if (Frame.CanGoBack)
                    Frame.GoBack();
            }
            catch (Exception)
            {
                var dlgErr = new ContentDialog
                {
                    Title = "Erreur",
                    Content = "Une erreur est survenue lors de l'ajout du projet.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dlgErr.ShowAsync();
            }
        }


        private void ButtonAssigner_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageAssignationClient));
        }
    }
}