using InterfaceAdmin.Singletons;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using InterfaceProjet.Helpers;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageParametres : Page
    {
        private const string KEY_THEME = "AppTheme";

        public PageParametres()
        {
            this.InitializeComponent();
            this.Loaded += PageParametres_Loaded;
        }

        private void PageParametres_Loaded(object sender, RoutedEventArgs e)
        {
            ChargerTheme();
            AfficherStatutConnexion();
        }

        private void ChargerTheme()
        {
            if (LocalSettingsHelper.ContainsKey(KEY_THEME))
            {
                string? theme = LocalSettingsHelper.GetValue(KEY_THEME);

                // Sélectionner le bon bouton radio
                foreach (RadioButton rb in rbTheme.Items)
                {
                    if (rb.Tag?.ToString() == theme)
                    {
                        rb.IsChecked = true;
                        break;
                    }
                }
            }
            else
            {
                // Par défaut : utiliser le thème sombre/système
                ((RadioButton)rbTheme.Items[2]).IsChecked = true;
            }
        }

        private void rbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (rbTheme.SelectedItem is RadioButton selectedRadio)
            {
                string? theme = selectedRadio.Tag?.ToString();

                if (!string.IsNullOrEmpty(theme))
                {
                    // Sauvegarder le choix
                    LocalSettingsHelper.SetValue(KEY_THEME, theme);

                    AppliquerTheme(theme);
                }
            }
        }

        private void AppliquerTheme(string theme)
        {
            if (this.XamlRoot?.Content is FrameworkElement rootElement)
            {
                switch (theme)
                {
                    case "Light":
                        rootElement.RequestedTheme = ElementTheme.Light;
                        break;

                    case "Dark":
                        rootElement.RequestedTheme = ElementTheme.Dark;
                        break;

                    case "Default":
                    default:
                        rootElement.RequestedTheme = ElementTheme.Default;
                        break;
                }
            }
        }

        private void AfficherStatutConnexion()
        {
            bool estConnecte = SingletonAdmin.getInstance().EstConnecte();

            if (estConnecte)
            {
                var admin = SingletonAdmin.getInstance().AdministrateurConnecte;
                if (admin != null)
                {
                    txtStatut.Text = $"Connecté en tant que : {admin.NomUtilisateur}";
                    iconStatut.Symbol = Symbol.ContactInfo;
                    iconStatut.Foreground = new SolidColorBrush(Colors.Green);
                    btnDeconnexion.Visibility = Visibility.Visible;
                }
            }
            else
            {
                txtStatut.Text = "Aucun administrateur connecté";
                iconStatut.Symbol = Symbol.Contact;
                iconStatut.Foreground = new SolidColorBrush(Colors.Gray);
                btnDeconnexion.Visibility = Visibility.Collapsed;
            }
        }

        private async void btnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            var admin = SingletonAdmin.getInstance().AdministrateurConnecte;
            string nomAdmin = admin != null ? admin.NomUtilisateur : "l'administrateur";

            var confirm = new ContentDialog
            {
                Title = "Confirmation de déconnexion",
                Content = $"Voulez-vous vraiment vous déconnecter en tant que {nomAdmin} ?",
                PrimaryButtonText = "Déconnexion",
                CloseButtonText = "Annuler",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.Content.XamlRoot
            };

            var result = await confirm.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                SingletonAdmin.getInstance().Deconnecter();
                AfficherStatutConnexion();

                var succes = new ContentDialog
                {
                    Title = "Déconnexion réussie",
                    Content = "Vous avez été déconnecté avec succès.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await succes.ShowAsync();
            }
        }
    }
}
