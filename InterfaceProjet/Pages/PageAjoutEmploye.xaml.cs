using InterfaceEmploye.Singletons;
using InterfaceProjet.Classes;
using InterfaceProjet.Singletons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageAjoutEmploye : Page
    {
        public PageAjoutEmploye()
        {
            InitializeComponent();
        }

        private void ButtonAnnuler_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private async void ButtonEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            // 1) Validation des champs
            if (!Validation())
            {
                var dlgErreur = new ContentDialog
                {
                    Title = "Formulaire incomplet",
                    Content = "Veuillez corriger les erreurs indiquées en rouge.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await dlgErreur.ShowAsync();
                return;
            }

            // 2) Si tout est valide, on récupère les valeurs
            string nom = tbxNom.Text.Trim();
            string prenom = tbxPrenom.Text.Trim();
            DateTime dateNaissance = dpDateNaissance.Date.DateTime;
            DateTime dateEmbauche = dpDateEmbauche.Date.DateTime;
            string email = tbxEmail.Text.Trim();
            string adresse = tbxAdresse.Text.Trim();
            decimal tauxHoraire = (decimal)nbrTauxHoraire.Value;
            string photoUrl = photoIdentite.Text.Trim();
            string statut = tglStatut.IsOn ? "Permanent" : "Journalier";

            // Génération du matricule (2 lettres du nom + année naissance + nombre 10–99)
            string prefixNom = nom.Length >= 2 ? nom[..2].ToUpper() : nom.ToUpper();
            Random rnd = new Random();
            int rand = rnd.Next(10, 100);
            string matricule = $"{prefixNom}-{dateNaissance.Year}-{rand}";

            try
            {
                // 3) Appel du singleton pour ajouter en BD
                SingletonEmploye.getInstance().AjouterEmploye(
                        nom,
                        prenom,
                        dateNaissance,
                        email,
                        adresse,
                        dateEmbauche,
                        tauxHoraire,
                        photoUrl,
                        statut
                    );

                var dlgOK = new ContentDialog
                {
                    Title = "Succès",
                    Content = "L'employé a été ajouté avec succès.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await dlgOK.ShowAsync();

                if (Frame.CanGoBack)
                    Frame.GoBack();
            }
            catch (Exception ex)
            {
                var dlgErreurBD = new ContentDialog
                {
                    Title = "Erreur",
                    Content = "Une erreur est survenue lors de l'ajout de l'employé.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await dlgErreurBD.ShowAsync();
            
            }
        }

        private bool Validation()
        {
            bool valid = true;

            // Effacer les anciens messages
            tblErrNom.Text = "";
            tblErrPOrenom.Text = "";
            tblErrDateNaissance.Text = "";
            tblErrEmail.Text = "";
            tblErrAdresse.Text = "";
            tblErrDateEmbauche.Text = "";
            tblErrTauxHoraire.Text = "";
            tblErrPhotoIdentite.Text = "";

            // NOM
            if (string.IsNullOrWhiteSpace(tbxNom.Text))
            {
                tblErrNom.Text = "Le nom est obligatoire.";
                valid = false;
            }

            // PRÉNOM
            if (string.IsNullOrWhiteSpace(tbxPrenom.Text))
            {
                tblErrPOrenom.Text = "Le prénom est obligatoire.";
                valid = false;
            }

            // DATE DE NAISSANCE
            DateTime dateNaissance = dpDateNaissance.Date.DateTime;
            int age = DateTime.Now.Year - dateNaissance.Year;
            if (dateNaissance.Date > DateTime.Now.AddYears(-age)) age--;

            if (age < 18 || age > 65)
            {
                tblErrDateNaissance.Text = "L'employé doit avoir entre 18 et 65 ans.";
                valid = false;
            }

            // EMAIL
            string email = tbxEmail.Text.Trim();
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                tblErrEmail.Text = "Entrez un courriel valide.";
                valid = false;
            }

            // ADRESSE
            if (string.IsNullOrWhiteSpace(tbxAdresse.Text))
            {
                tblErrAdresse.Text = "L'adresse est obligatoire.";
                valid = false;
            }

            // DATE D'EMBAUCHE
            DateTime dateEmbauche = dpDateEmbauche.Date.DateTime;
            if (dateEmbauche > DateTime.Now.Date.AddYears(1))
              
            {
                tblErrDateEmbauche.Text = "La date d'embauche ne peut pas être dans le futur.";
                valid = false;
            }

            // TAUX HORAIRE
            double tauxDouble = nbrTauxHoraire.Value;
            if (tauxDouble < 15)
            {
                tblErrTauxHoraire.Text = "Le taux horaire doit être d'au moins 15$.";
                valid = false;
            }

            // PHOTO IDENTITÉ
            string photoUrl = photoIdentite.Text.Trim();
            if (string.IsNullOrWhiteSpace(photoUrl) ||
                !Uri.IsWellFormedUriString(photoUrl, UriKind.Absolute))
            {
                tblErrPhotoIdentite.Text = "Entrez une URL de photo valide.";
                valid = false;
            }

            return valid;
        }
    }
}

