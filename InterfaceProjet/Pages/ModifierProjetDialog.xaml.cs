using InterfaceProjet.Classes;
using InterfaceProjet.Singletons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Globalization;
//a revoir
namespace InterfaceProjet.Pages
{
    public sealed partial class ModifierProjetDialog : ContentDialog
    {
        private readonly Projet _projetOriginal;

        public ModifierProjetDialog(Projet projet)
        {
            this.InitializeComponent();

            _projetOriginal = projet ?? throw new ArgumentNullException(nameof(projet));

            // Pré-remplir les champs
            tbNumero.Text = _projetOriginal.NumeroProjet;
            tbTitre.Text = _projetOriginal.Titre;
            tbDescription.Text = _projetOriginal.Description;
            tbBudget.Text = _projetOriginal.Budget.ToString(CultureInfo.InvariantCulture);
            tbNbEmployes.Text = _projetOriginal.NbEmployesRequis.ToString();

            // Date début
            dpDateDebut.Date = _projetOriginal.DateDebut;

            // Statut
            if (_projetOriginal.Statut?.ToLower().Contains("termin") == true)
                cbStatut.SelectedIndex = 1;
            else
                cbStatut.SelectedIndex = 0;
        }

        private void ResetErreurs()
        {
            tbTitreErreur.Visibility = Visibility.Collapsed;
            tbDateErreur.Visibility = Visibility.Collapsed;
            tbBudgetErreur.Visibility = Visibility.Collapsed;
            tbNbEmployesErreur.Visibility = Visibility.Collapsed;
            tbStatutErreur.Visibility = Visibility.Collapsed;
            tbDescriptionErreur.Visibility = Visibility.Collapsed;

            tbTitreErreur.Text = "";
            tbDateErreur.Text = "";
            tbBudgetErreur.Text = "";
            tbNbEmployesErreur.Text = "";
            tbStatutErreur.Text = "";
            tbDescriptionErreur.Text = "";
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ResetErreurs();
            bool ok = true;

            string titre = tbTitre.Text.Trim();
            string description = tbDescription.Text.Trim();
            string statut = (cbStatut.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            // Titre
            if (string.IsNullOrWhiteSpace(titre))
            {
                tbTitreErreur.Text = "Le titre est obligatoire.";
                tbTitreErreur.Visibility = Visibility.Visible;
                ok = false;
            }

            // Date
            DateTime dateDebut;
            if (dpDateDebut.Date == null)
            {
                tbDateErreur.Text = "Veuillez choisir une date.";
                tbDateErreur.Visibility = Visibility.Visible;
                ok = false;
                dateDebut = DateTime.Now;
            }
            else
            {
                dateDebut = dpDateDebut.Date.DateTime;
            }

            // Budget
            decimal budget;
            if (!decimal.TryParse(tbBudget.Text.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out budget) || budget <= 0)
            {
                tbBudgetErreur.Text = "Budget invalide.";
                tbBudgetErreur.Visibility = Visibility.Visible;
                ok = false;
            }

            // Nb employés
            int nbEmployes;
            if (!int.TryParse(tbNbEmployes.Text, out nbEmployes) || nbEmployes <= 0 || nbEmployes > 5)
            {
                tbNbEmployesErreur.Text = "Entrez un nombre entre 1 et 5.";
                tbNbEmployesErreur.Visibility = Visibility.Visible;
                ok = false;
            }

            // Statut
            if (string.IsNullOrWhiteSpace(statut))
            {
                tbStatutErreur.Text = "Veuillez choisir un statut.";
                tbStatutErreur.Visibility = Visibility.Visible;
                ok = false;
            }

            // Description
            if (string.IsNullOrWhiteSpace(description))
            {
                tbDescriptionErreur.Text = "La description est obligatoire.";
                tbDescriptionErreur.Visibility = Visibility.Visible;
                ok = false;
            }

            if (!ok)
            {
                args.Cancel = true; // ne pas fermer si invalide
                return;
            }

            // Appel au singleton pour mettre à jour en BD
            var singleton = SingletonProjet.getInstance();

            // On réutilise IdClient du projet original (ta vue de BD doit le fournir)
            // Ne touche PAS au client lors de la modification
            singleton.modifierProjetSansClient(
                numeroProjet: _projetOriginal.NumeroProjet,
                titre: titre,
                dateDebut: dateDebut,
                description: description,
                budget: budget,
                nbEmployesRequis: nbEmployes,
                statut: statut
            );

            // Mise à jour de l'objet local
            _projetOriginal.Titre = titre;
            _projetOriginal.Description = description;
            _projetOriginal.Budget = budget;
            _projetOriginal.NbEmployesRequis = nbEmployes;
            _projetOriginal.Statut = statut;
            _projetOriginal.DateDebut = dateDebut;
        }
// ?? Ne touche PAS à IdClient !
        private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            // Si l’utilisateur clique sur Enregistrer mais que la validation a échoué
            // (ok == false), on laisse ContentDialog_PrimaryButtonClick gérer args.Cancel.
        }
    }
}
