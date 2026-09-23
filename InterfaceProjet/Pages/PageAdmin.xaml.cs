using InterfaceAdmin.Singletons;
using InterfaceProjet.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace InterfaceProjet.Pages
{
    /// <summary>
    /// Page d'initialisation et de création du compte administrateur principal.
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

            // 1. Vérification des champs obligatoires
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(pass) ||
                string.IsNullOrWhiteSpace(confirm))
            {
                ErrorText.Text = "Veuillez remplir tous les champs.";
                return;
            }

            // 2. Vérification de confirmation du mot de passe
            if (pass != confirm)
            {
                ErrorText.Text = "Les mots de passe ne correspondent pas.";
                return;
            }

            // 3. Hachage SHA-256 du mot de passe
            string hash = Cryptage.GenererSHA256(pass);

            try
            {
                // 4. Enregistrement et connexion de l'administrateur
                SingletonAdmin.getInstance().CreerAdministrateur(username, hash);
                bool ok = SingletonAdmin.getInstance().ConnecterAdministrateur(username, hash);

                if (!ok)
                {
                    ErrorText.Text = "Impossible de connecter l'administrateur.";
                    return;
                }

                // 5. Activation de la navigation sur la fenêtre principale
                if (App.fenetrePrincipale is MainWindow mw)
                {
                    mw.ActiverNavigation();
                }
                else
                {
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
