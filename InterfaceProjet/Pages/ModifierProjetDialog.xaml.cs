using InterfaceProjet.Classes;
using InterfaceProjet.Singletons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace InterfaceProjet.Pages
{
    public sealed partial class ModifierProjetDialog : ContentDialog
    {
        private Projet projetModifier;
        public bool VeutChangerClient  = false;

        public ModifierProjetDialog(Projet projet)
        {
            this.InitializeComponent();

            projetModifier = projet;

            if (projetModifier != null)
            {
                tbTitre.Text = projetModifier.Titre;
                tbDescription.Text = projetModifier.Description;
                NbBudget.Text = projetModifier.Budget.ToString();
                NbEmployes.Text = projetModifier.NbEmployesRequis.ToString();

                dpDateDebut.Date = new DateTimeOffset(projetModifier.DateDebut);

                cbStatut.SelectedItem = projetModifier.Statut;

                // client actuel
                if (projetModifier.IdClient > 0)
                    tbClient.Text = $"{projetModifier.IdClient} - {projetModifier.NomClient}";
                else
                    tbClient.Text = "Aucun client assigné";
            }
        }


        private void ResetErreurs()
        {
            tbTitreErreur.Text = "";
            tbTitreErreur.Visibility = Visibility.Collapsed;

            tbDateErreur.Text = "";
            tbDateErreur.Visibility = Visibility.Collapsed;

            tbBudgetErreur.Text = "";
            tbBudgetErreur.Visibility = Visibility.Collapsed;

            tbNbEmployesErreur.Text = "";
            tbNbEmployesErreur.Visibility = Visibility.Collapsed;

            tbStatutErreur.Text = "";
            tbStatutErreur.Visibility = Visibility.Collapsed;

            tbDescriptionErreur.Text = "";
            tbDescriptionErreur.Visibility = Visibility.Collapsed;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ResetErreurs();
            bool valide = true;

            string titre = tbTitre.Text.Trim();
            string description = tbDescription.Text.Trim();
            string statut = cbStatut.SelectedItem.ToString();

            DateTime dateDebut = dpDateDebut.Date.DateTime;

            decimal budget = (decimal)NbBudget.Value;
            int nbEmployes = (int)NbEmployes.Value;


            if (string.IsNullOrWhiteSpace(titre))
            {
                tbTitreErreur.Text = "Le titre est obligatoire.";
                tbTitreErreur.Visibility = Visibility.Visible;
                valide = false;
            }

            if (dateDebut.Year < 2000)
            {
                tbDateErreur.Text = "Choisissez une date valide.";
                tbDateErreur.Visibility = Visibility.Visible;
                valide = false;
            }

            if (budget <= 0)
            {
                tbBudgetErreur.Text = "Entrez un budget positif.";
                tbBudgetErreur.Visibility = Visibility.Visible;
                valide = false;
            }

            if (nbEmployes < 1 || nbEmployes > 5)
            {
                tbNbEmployesErreur.Text = "Le nombre d'employés doit être entre 1 et 5.";
                tbNbEmployesErreur.Visibility = Visibility.Visible;
                valide = false;
            }


            if (string.IsNullOrWhiteSpace(description))
            {
                tbDescriptionErreur.Text = "La description est obligatoire.";
                tbDescriptionErreur.Visibility = Visibility.Visible;
                valide = false;
            }


            if (!valide)
            {
                args.Cancel = true;
                return;
            }

            projetModifier.Titre = titre;
            projetModifier.Description = description;
            projetModifier.Budget = budget;
            projetModifier.NbEmployesRequis = nbEmployes;
            projetModifier.Statut = statut;
            projetModifier.DateDebut = dateDebut;

            SingletonProjet.getInstance().modifierProjetSansClient(
                numeroProjet: projetModifier.NumeroProjet,
                titre: titre,
                dateDebut: dateDebut,
                description: description,
                budget: budget,
                nbEmployesRequis: nbEmployes,
                statut: statut
            );
        }

        private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {

        }
        private void BtnChangerClient_Click(object sender, RoutedEventArgs e)
        {
            VeutChangerClient = true;

            // On ferme la boîte de dialogue
            this.Hide();
        }

    }
}