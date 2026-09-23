using InterfaceClient.Singletons;
using InterfaceProjet.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Text.RegularExpressions;

namespace InterfaceProjet.Pages
{
    /// <summary>
    /// Boîte de dialogue permettant la modification des coordonnées d'un client.
    /// </summary>
    public sealed partial class ModifierClientDialog : ContentDialog
    {
        private Client _client;

        public ModifierClientDialog(Client client)
        {
            this.InitializeComponent();
            _client = client ?? throw new ArgumentNullException(nameof(client));

            // Pré-remplir les champs
            tbNom.Text = _client.Nom;
            tbAdresse.Text = _client.Adresse;
            tbTelephone.Text = _client.Telephone;
            tbEmail.Text = _client.Email;
        }

        private void ResetErreurs()
        {
            tblErrNom.Text = "";
            tblErrAdresse.Text = "";
            tblErrTelephone.Text = "";
            tblErrEmail.Text = "";

            tblErrNom.Visibility = Visibility.Collapsed;
            tblErrAdresse.Visibility = Visibility.Collapsed;
            tblErrTelephone.Visibility = Visibility.Collapsed;
            tblErrEmail.Visibility = Visibility.Collapsed;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ResetErreurs();
            bool valide = true;

            string nom = tbNom.Text.Trim();
            string adresse = tbAdresse.Text.Trim();
            string telephone = tbTelephone.Text.Trim();
            string email = tbEmail.Text.Trim();

            // Nom
            if (string.IsNullOrWhiteSpace(nom))
            {
                tblErrNom.Text = "Le nom est obligatoire.";
                tblErrNom.Visibility = Visibility.Visible;
                valide = false;
            }

            // Adresse
            if (string.IsNullOrWhiteSpace(adresse))
            {
                tblErrAdresse.Text = "L'adresse est obligatoire.";
                tblErrAdresse.Visibility = Visibility.Visible;
                valide = false;
            }

            // Téléphone
            var regexTel = new Regex(@"^[0-9+\-\s]+$");
            if (string.IsNullOrWhiteSpace(telephone) || !regexTel.IsMatch(telephone))
            {
                tblErrTelephone.Text = "Format de téléphone invalide.";
                tblErrTelephone.Visibility = Visibility.Visible;
                valide = false;
            }

            // Courriel
            if (string.IsNullOrWhiteSpace(email))
            {
                tblErrEmail.Text = "L'email est obligatoire.";
                tblErrEmail.Visibility = Visibility.Visible;
                valide = false;
            }
            else
            {
                var regexMail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!regexMail.IsMatch(email))
                {
                    tblErrEmail.Text = "Format d'email invalide.";
                    tblErrEmail.Visibility = Visibility.Visible;
                    valide = false;
                }
            }

            if (!valide)
            {
                args.Cancel = true;
                return;
            }

            // Mise à jour dans SQLite via le singleton
            SingletonClient.getInstance()
                           .ModifierClientAvecProcedure(_client.IdClient, nom, adresse, telephone, email);

            // Mise à jour de l'objet en mémoire
            _client.Nom = nom;
            _client.Adresse = adresse;
            _client.Telephone = telephone;
            _client.Email = email;
        }
    }
}
