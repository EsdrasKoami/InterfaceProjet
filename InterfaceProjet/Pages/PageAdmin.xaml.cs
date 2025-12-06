using InterfaceAdmin.Singletons;
using InterfaceProjet.Helpers;
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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace InterfaceProjet.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageAdmin : Page
    {
        public PageAdmin()
        {
            InitializeComponent();
        }
    
     private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string pass = PasswordBox.Password;
            string confirm = ConfirmPasswordBox.Password;

            ErrorText.Text = string.Empty;

            // 1. Champs vides
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(pass) ||
                string.IsNullOrWhiteSpace(confirm))
            {
                ErrorText.Text = "Veuillez remplir tous les champs.";
                return;
            }

            // 2. Confirmation mot de passe
            if (pass != confirm)
            {
                ErrorText.Text = "Les mots de passe ne correspondent pas.";
                return;
            }

            // 3. Hash SHA256 du mot de passe
            string hash = Cryptage.GenererSHA256(pass);

            try
            {
                // 4. Enregistrer + connecter l'admin
                SingletonAdmin.getInstance().CreerAdministrateur(username, hash);
                bool ok = SingletonAdmin.getInstance().ConnecterAdministrateur(username, hash);

                if (!ok)
                {
                    ErrorText.Text = "Impossible de connecter l'administrateur.";
                    return;
                }

                // 5. Succès ? activer la navigation dans la fenêtre principale
                if (App.fenetrePrincipale is MainWindow mw)
                {
                    mw.ActiverNavigation();
                }
                else
                {
                    // Plan B : revenir à l'accueil via le Frame local (normalement pas nécessaire)
                    Frame.Navigate(typeof(PageAccueil));
                }
            }
            catch (System.Exception ex)
            {
                ErrorText.Text = ex.Message;
            }
        }
    }
}
