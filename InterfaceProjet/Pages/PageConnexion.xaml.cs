using InterfaceAdmin.Singletons;
using InterfaceProjet.Helpers;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageConnexion : Page
    {
        public PageConnexion()
        {
            InitializeComponent();
            this.Loaded += PageConnexion_Loaded;
        }

        private void PageConnexion_Loaded(object sender, RoutedEventArgs e)
        {
            AfficherStatutConnexion();

            // Focus sur le champ nom d'utilisateur au chargement
            txtUsername.Focus(FocusState.Programmatic);
        }

        // Afficher le statut de connexion actuel
        private void AfficherStatutConnexion()
        {
            bool estConnecte = SingletonAdmin.getInstance().EstConnecte();

            if (estConnecte)
            {
                // Administrateur déjà connecté
                var admin = SingletonAdmin.getInstance().AdministrateurConnecte;
                if (admin != null)
                {
                    txtStatut.Text = $"Connecté en tant que : {admin.NomUtilisateur}";
                    iconStatut.Symbol = Symbol.ContactInfo;
                    iconStatut.Foreground = new SolidColorBrush(Colors.Green);

                    // Ajuster les boutons en session active
                    btnConnexion.Content = "Déjà connecté";
                    btnConnexion.IsEnabled = false;
                    btnAnnuler.Content = "Retour à l'accueil";
                }
            }
            else
            {
                // Aucun administrateur connecté
                txtStatut.Text = "Aucun administrateur connecté";
                iconStatut.Symbol = Symbol.Contact;
                iconStatut.Foreground = new SolidColorBrush(Colors.Gray);

                btnConnexion.Content = "Se connecter";
                btnConnexion.IsEnabled = true;
                btnAnnuler.Content = "Annuler";
            }
        }

        // Bouton "Se connecter"
        private void btnConnexion_Click(object sender, RoutedEventArgs e)
        {
            TenterConnexion();
        }

        // Permettre la validation par la touche Entrée
        private void Input_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                TenterConnexion();
                e.Handled = true;
            }
        }

        private async void TenterConnexion()
        {
            errorBorder.Visibility = Visibility.Collapsed;
            successBorder.Visibility = Visibility.Collapsed;

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(username))
            {
                AfficherErreur("Veuillez entrer un nom d'utilisateur.");
                txtUsername.Focus(FocusState.Programmatic);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                AfficherErreur("Veuillez entrer un mot de passe.");
                txtPassword.Focus(FocusState.Programmatic);
                return;
            }

            try
            {
                string passwordHash = Cryptage.GenererSHA256(password);

                bool connexionReussie = SingletonAdmin.getInstance().ConnecterAdministrateur(username, passwordHash);

                if (connexionReussie)
                {
                    var admin = SingletonAdmin.getInstance().AdministrateurConnecte;
                    string nomAdmin = admin != null ? admin.NomUtilisateur : username;
                    AfficherSucces($"Connexion réussie ! Bienvenue {nomAdmin}.");

                    txtUsername.Text = string.Empty;
                    txtPassword.Password = string.Empty;

                    // Mettre à jour l'affichage
                    AfficherStatutConnexion();

                    // Temporisation avant redirection
                    await System.Threading.Tasks.Task.Delay(1500);

                    // Rediriger vers l'accueil
                    Frame.Navigate(typeof(PageAccueil));
                }
                else
                {
                    // Échec d'authentification
                    AfficherErreur("Nom d'utilisateur ou mot de passe incorrect.");
                    txtPassword.Password = string.Empty;
                    txtPassword.Focus(FocusState.Programmatic);
                }
            }
            catch (Exception ex)
            {
                AfficherErreur($"Erreur lors de la connexion : {ex.Message}");
                txtPassword.Password = string.Empty;
            }
        }

        // Bouton "Annuler" ou "Retour à l'accueil"
        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            txtUsername.Text = string.Empty;
            txtPassword.Password = string.Empty;

            errorBorder.Visibility = Visibility.Collapsed;
            successBorder.Visibility = Visibility.Collapsed;

            Frame.Navigate(typeof(PageAccueil));
        }

        private void AfficherErreur(string message)
        {
            txtError.Text = message;
            errorBorder.Visibility = Visibility.Visible;
            successBorder.Visibility = Visibility.Collapsed;
        }

        private void AfficherSucces(string message)
        {
            txtSuccess.Text = message;
            successBorder.Visibility = Visibility.Visible;
            errorBorder.Visibility = Visibility.Collapsed;
        }
    }
}
