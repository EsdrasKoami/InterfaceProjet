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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace InterfaceProjet.Pages
{
    public sealed partial class PageAjoutPojet : Page
    {
        public PageAjoutPojet()
        {
            InitializeComponent();
        }

        private void ButtonAnnuler_Click(object sender, RoutedEventArgs e)
        {
            // Retour à la page précédente si possible
            if (this.Frame != null && this.Frame.CanGoBack)
                this.Frame.GoBack();
        }

        private void ButtonEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            // On réinitialise les messages d’erreur
            tblErrTitre.Text = "";
            tblErrDateDebut.Text = "";
            tblErrDescription.Text = "";
            tblErrBudget.Text = "";
            tblErrNbrEmploye.Text = "";
            tblErrTotalsalaire.Text = "";
            tblErrIdClient.Text = "";

            bool valide = true;

            // 1) RÉCUPÉRATION DES VALEURS
            string titre = tbxtitre.Text.Trim();
            string description = tbxDescription.Text.Trim();

            // Date
            DateTime dateDebut = DateTime.Now;
            if (dpDateDebut.Date != null)
            {
                dateDebut = dpDateDebut.Date.DateTime;
            }

            // Budget
            double budgetValeur = nbxBudget.Value;

            // Nombre d’employés
            double nbEmployeValeur = nbrEmploye.Value;

            // Total salaires (facultatif : souvent 0 au début)
            double totalSalaireValeur = totalSalaire.Value;

            // Id client
            string idClientTexte = idClient.Text.Trim();
            int idClientInt = 0;

            // 2) VALIDATIONS

            // Titre
            if (string.IsNullOrWhiteSpace(titre))
            {
                tblErrTitre.Text = "Entrez un titre valide.";
                valide = false;
            }

            // Date
            if (dpDateDebut.Date == null)
            {
                tblErrDateDebut.Text = "Choisissez une date de début.";
                valide = false;
            }

            // Description
            if (string.IsNullOrWhiteSpace(description))
            {
                tblErrDescription.Text = "La description est obligatoire.";
                valide = false;
            }

            // Budget
            if (budgetValeur <= 0)
            {
                tblErrBudget.Text = "Entrez un budget positif.";
                valide = false;
            }

            // Nb employés (1 à 5 selon l’énoncé)
            if (nbEmployeValeur <= 0 || nbEmployeValeur > 5)
            {
                tblErrNbrEmploye.Text = "Le nombre d’employés doit être entre 1 et 5.";
                valide = false;
            }

            // Total salaires (optionnel, mais on peut vérifier qu’il n’est pas négatif)
            if (totalSalaireValeur < 0)
            {
                tblErrTotalsalaire.Text = "Le total des salaires ne peut pas être négatif.";
                valide = false;
            }

            // Id client (doit être un entier)
            if (!int.TryParse(idClientTexte, out idClientInt))
            {
                tblErrIdClient.Text = "Entrez un identifiant client valide (nombre).";
                valide = false;
            }

            // Si une erreur → on arrête là
            if (!valide)
                return;

            // 3) GÉNÉRER LE NUMÉRO DE PROJET
            // Format: idClient-XX-année (comme dans l’énoncé)
            Random rnd = new Random();
            int sequence = rnd.Next(1, 100); // 01 à 99
            string numeroProjet = $"{idClientInt}-{sequence:D2}-{dateDebut.Year}";

            // 4) APPEL AU SINGLETON POUR ENREGISTRER EN BD
            SingletonProjet.getInstance().ajouterProjetAvecProcedure(
                numeroProjet,
                titre,
                dateDebut,
                description,
                (decimal)budgetValeur,
                (int)nbEmployeValeur,
                idClientInt
            );

            // 5) RETOUR À LA PAGE PRÉCÉDENTE OU CLEAR LES CHAMPS
            if (this.Frame != null && this.Frame.CanGoBack)
                this.Frame.GoBack();
           
        }
    }
}