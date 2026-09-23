using InterfaceEmploye.Singletons;
using InterfaceProjet.Classes;
using Microsoft.UI.Xaml.Controls;
using System;

namespace InterfaceProjet.Pages
{
    public sealed partial class ModifierEmployeDialog : ContentDialog
    {
        private Employe Emp;

        public ModifierEmployeDialog(Employe emp)
        {
            InitializeComponent();
            Emp = emp ?? throw new ArgumentNullException(nameof(emp));

            // Pré-remplir les champs
            tbNom.Text = Emp.Nom;
            tbPrenom.Text = Emp.Prenom;
            tbEmail.Text = Emp.Email;
            tbAdresse.Text = Emp.Adresse;
            nbTauxHoraire.Value = (double)Emp.TauxHoraire;
            tglStatut.IsOn = Emp.Statut == "Permanent";
            tbPhotoUrl.Text = Emp.PhotoUrl;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Réinitialiser les messages d'erreur
            tblErrNom.Text = "";
            tblErrPrenom.Text = "";
            tblErrEmail.Text = "";
            tblErrAdresse.Text = "";
            tblErrTaux.Text = "";
            tblErrPhoto.Text = "";

            bool valide = true;

            string nom = tbNom.Text.Trim();
            string prenom = tbPrenom.Text.Trim();
            string email = tbEmail.Text.Trim();
            string adresse = tbAdresse.Text.Trim();
            double tauxDouble = nbTauxHoraire.Value;
            string photoUrl = tbPhotoUrl.Text.Trim();

            // Nom
            if (string.IsNullOrWhiteSpace(nom))
            {
                tblErrNom.Text = "Entrez un nom.";
                valide = false;
            }

            // Prénom
            if (string.IsNullOrWhiteSpace(prenom))
            {
                tblErrPrenom.Text = "Entrez un prénom.";
                valide = false;
            }

            // Courriel
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                tblErrEmail.Text = "Entrez un courriel valide.";
                valide = false;
            }

            // Adresse
            if (string.IsNullOrWhiteSpace(adresse))
            {
                tblErrAdresse.Text = "Entrez une adresse.";
                valide = false;
            }

            // Taux horaire
            if (tauxDouble < 15)
            {
                tblErrTaux.Text = "Taux horaire minimum : 15 $.";
                valide = false;
            }

            // Validation de l'URL de la photo
            if (string.IsNullOrWhiteSpace(photoUrl))
            {
                tblErrPhoto.Text = "Entrez un lien d'image (URL).";
                valide = false;
            }
            else
            {
                if (!Uri.IsWellFormedUriString(tbPhotoUrl.Text, UriKind.Absolute))
                {
                    tblErrPhoto.Text = "Lien d'image invalide. Utilisez une URL http ou https.";
                    valide = false;
                }
            }

            if (!valide)
            {
                args.Cancel = true;
                return;
            }

            decimal taux = (decimal)tauxDouble;
            string statut = tglStatut.IsOn ? "Permanent" : "Journalier";

            // Mise à jour dans SQLite via le singleton
            SingletonEmploye.getInstance().ModifierEmploye(
                Emp.Matricule,
                nom,
                prenom,
                email,
                adresse,
                taux,
                photoUrl,
                statut
            );

            // Mise à jour de l'objet en mémoire
            Emp.Nom = nom;
            Emp.Prenom = prenom;
            Emp.Email = email;
            Emp.Adresse = adresse;
            Emp.TauxHoraire = taux;
            Emp.Statut = statut;
            Emp.PhotoUrl = photoUrl;
        }
    }
}
