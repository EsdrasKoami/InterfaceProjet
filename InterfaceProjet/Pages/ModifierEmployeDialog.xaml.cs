using InterfaceEmploye.Singletons;
using InterfaceProjet.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace InterfaceProjet.Pages
{
    public sealed partial class ModifierEmployeDialog : ContentDialog
    {

        private Employe Emp;

        public ModifierEmployeDialog(Employe emp)
        {
            InitializeComponent();
            Emp = emp;

            // Pré-remplir les champs
            tbNom.Text = Emp.Nom;
            tbPrenom.Text = Emp.Prenom;
            tbEmail.Text = Emp.Email;
            tbAdresse.Text = Emp.Adresse;
            nbTauxHoraire.Value = (double)Emp.TauxHoraire;
            tglStatut.IsOn = Emp.Statut == "Permanent";
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Réinitialiser les messages d'erreur
            tblErrNom.Text = "";
            tblErrPrenom.Text = "";
            tblErrEmail.Text = "";
            tblErrAdresse.Text = "";
            tblErrTaux.Text = "";

            bool valide = true;

            string nom = tbNom.Text.Trim();
            string prenom = tbPrenom.Text.Trim();
            string email = tbEmail.Text.Trim();
            string adresse = tbAdresse.Text.Trim();
            double tauxDouble = nbTauxHoraire.Value;

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

            // Email
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

            // Si erreurs → on ne ferme pas le dialog
            if (!valide)
            {
                args.Cancel = true;
                return;
            }

            decimal taux = (decimal)tauxDouble;
            string statut = tglStatut.IsOn ? "Permanent" : "Journalier";

            // Mise à jour en BD via le singleton
            SingletonEmploye.getInstance().ModifierEmploye(
                Emp.Matricule,
                nom,
                prenom,
                email,
                adresse,
                taux,
                Emp.PhotoUrl,
                statut
            );

            // Mise à jour de l’objet en mémoire
            Emp.Nom = nom;
            Emp.Prenom = prenom;
            Emp.Email = email;
            Emp.Adresse = adresse;
            Emp.TauxHoraire = taux;
            Emp.Statut = statut;
        }
    }
}
