using InterfaceClient.Singletons;
using InterfaceProjet.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace InterfaceProjet.Pages
{
    public sealed partial class ModifierClientDialog : ContentDialog
    {
        private Client _client;   // client à modifier

        public ModifierClientDialog(Client client)
        {
            this.InitializeComponent();
            _client = client;

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

            // Téléphone (validation simple, comme dans la page d'ajout)
            var regexTel = new Regex(@"^[0-9+\-\s]+$");
            if (string.IsNullOrWhiteSpace(telephone) || !regexTel.IsMatch(telephone))
            {
                tblErrTelephone.Text = "Format de téléphone invalide.";
                tblErrTelephone.Visibility = Visibility.Visible;
                valide = false;
            }

            // Email
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
                // Empêche la fermeture si les données sont invalides
                args.Cancel = true;
                return;
            }

            // --- Mise à jour en BD via le singleton ---
            // Adapte le nom de la méthode à ton SingletonClient
            // Exemple : ModifierClientAvecProcedure(int id, string nom, string adresse, string tel, string email)
            SingletonClient.getInstance()
                           .ModifierClientAvecProcedure(_client.IdClient, nom, adresse, telephone, email);

            // --- Mise à jour de l'objet en mémoire pour rafraîchir la liste ---
            _client.Nom = nom;
            _client.Adresse = adresse;
            _client.Telephone = telephone;
            _client.Email = email;
        }
    }
}