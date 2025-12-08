using InterfaceAdmin.Singletons;
using InterfaceClient.Singletons;
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



namespace InterfaceProjet.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageAjoutClient : Page
    {
        public PageAjoutClient()
        {
            InitializeComponent();
            if (!SingletonAdmin.getInstance().EstConnecte())
            {
                btnEnregister.IsEnabled = false;
                btnEnregister.Opacity = 0.5;


            }
        }
        private void ButtonAnnuler_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private  async void ButtonEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (!Validation())
            {
                return;
                string nom = tbxNom.Text.Trim();
                string adresse = tbxAdresse.Text.Trim();
                string telephone = tbxTelephone.Text.Trim();
                string email = tbxEmail.Text.Trim();
            }
            try
            {
                SingletonClient.getInstance().AjouterClientAvecProcedure(tbxNom.Text.Trim(), tbxAdresse.Text.Trim(), tbxTelephone.Text.Trim(), tbxEmail.Text.Trim());

                _ = new ContentDialog
                {
                    Title = "Succès",
                    Content = "Le client a été ajouté avec succès.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                }.ShowAsync();

                if (Frame.CanGoBack)
                    Frame.GoBack();

            }
            catch (Exception ex)
            {
                _ = new ContentDialog
                {
                    Title = "Erreur",
                    Content = "Une erreur est survenue lors de l'ajout du client : " + ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                }.ShowAsync();
            }
        }
     public bool Validation()
        {
            bool valid =  true;

            // Vide les textBox
            tblErrNom.Text = "";
            tblErrAdresse.Text = "";
            tblErrNumeroTelephone.Text = "";
            tblErrEmail.Text = "";

            if (string.IsNullOrWhiteSpace(tbxNom.Text))
            {
                tblErrNom.Text = "Le nom est obligatoire.";
                valid = false;
            }
            if (string.IsNullOrWhiteSpace(tbxAdresse.Text))
            {
                tblErrAdresse.Text = "L'adresse est obligatoire.";
                valid = false;
            }
            var regexTel = new Regex(@"^\d{3}-\d{3}-\d{4}$");

            if (!regexTel.IsMatch(tbxTelephone.Text.Trim()))
            {
                tblErrNumeroTelephone.Text = "Format attendu : 819-555-0101.";
                valid = false;
            }
            string email = tbxEmail.Text.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                tblErrEmail.Text = "L'email est obligatoire.";
                valid = false;
            }
            else
            {
              
                var regexMail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!regexMail.IsMatch(email))
                {
                    tblErrEmail.Text = "Format d'email invalide.";
                    valid = false;
                }
            }

            return valid;
        }
    
    }
    }


