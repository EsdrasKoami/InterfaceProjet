using InterfaceAdmin.Singletons;
using InterfaceProjet.Classes;
using InterfaceProjet.Pages;
using InterfaceProjet.Singletons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using InterfaceProjet.Helpers;
using Windows.Storage;
using WinRT.Interop;

namespace InterfaceProjet
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Charger le thème sauvegardé
            ChargerThemeSauvegarde();

            var maintenant = DateTime.Now;
            tbDate.Text = maintenant.ToString("dd/MM/yyyy");
            tbHeure.Text = maintenant.ToString("HH:mm");

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(titlebar);

            VerifierEtNaviguer();
        }

        // Charger le thème sauvegardé
        private void ChargerThemeSauvegarde()
        {
            string themeSauvegarde = LocalSettingsHelper.GetValue("AppTheme", "Dark") ?? "Dark";

            if (this.Content is FrameworkElement rootElement)
            {
                switch (themeSauvegarde)
                {
                    case "Dark":
                        rootElement.RequestedTheme = ElementTheme.Dark;
                        break;
                    case "Light":
                        rootElement.RequestedTheme = ElementTheme.Light;
                        break;
                    default:
                        rootElement.RequestedTheme = ElementTheme.Default;
                        break;
                }
            }
        }

        private void VerifierEtNaviguer()
        {
            bool adminExiste = SingletonAdmin.getInstance().AdministrateurExiste();

            if (!adminExiste)
                mainFrame.Navigate(typeof(PageAdmin));
            else
                ActiverNavigation();
        }

        public void ActiverNavigation()
        {
            mainFrame.Navigate(typeof(PageAccueil));
        }

        private async void navView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var premiereConnexion = SingletonAdmin.getInstance();

            if (!premiereConnexion.AdministrateurExiste())
            {
                ContentDialog dlg = new ContentDialog
                {
                    Title = "Première Connexion",
                    Content = "Afin de naviguer, vous devez créer un compte Administrateur.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await dlg.ShowAsync();
                return;
            }

            if (args.InvokedItemContainer is NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "accueil":
                        mainFrame.Navigate(typeof(PageAccueil));
                        break;

                    case "employe":
                        mainFrame.Navigate(typeof(PageEmployes));
                        break;

                    case "clients":
                        mainFrame.Navigate(typeof(PageClients));
                        break;

                    case "projets":
                        mainFrame.Navigate(typeof(PageProjets));
                        break;

                    case "connexion":
                        mainFrame.Navigate(typeof(PageConnexion));
                        break;

                    case "parametres":
                        mainFrame.Navigate(typeof(PageParametres));
                        break;

                    case "deconnexion":
                        await GererDeconnexion();
                        break;

                    default:
                        mainFrame.Navigate(typeof(PageAccueil));
                        break;
                }
            }
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void MenuExporter_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileSavePicker();
            var hWnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(picker, hWnd);

            picker.SuggestedFileName = "projets";
            picker.FileTypeChoices.Add("Fichier CSV", new List<string>() { ".csv" });

            StorageFile monFichier = await picker.PickSaveFileAsync();
            if (monFichier == null)
                return;

            var singleton = SingletonProjet.getInstance();
            singleton.getAllProjets();
            List<Projet> liste = singleton.Liste.ToList();

            var lignes = new List<string>();
            lignes.Add("NumeroProjet;Titre;NomClient;DateDebut;Budget;TotalSalaires;Statut");
            lignes.AddRange(liste.ConvertAll(p => p.ToString()));

            await FileIO.WriteLinesAsync(monFichier, lignes, Windows.Storage.Streams.UnicodeEncoding.Utf8);

            await AfficherDialogue("Exportation réussie", $"Les projets ont été exportés dans :\n{monFichier.Path}");
        }

        private async System.Threading.Tasks.Task GererDeconnexion()
        {
            if (!SingletonAdmin.getInstance().EstConnecte())
            {
                await AfficherDialogue("Aucune connexion active", "Vous n'êtes pas connecté en tant qu'administrateur.");
                return;
            }

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

                await AfficherDialogue("Déconnexion réussie", "Vous avez été déconnecté avec succès.");

                mainFrame.Navigate(typeof(PageAccueil));
            }
        }

        private async System.Threading.Tasks.Task AfficherDialogue(string titre, string contenu)
        {
            await System.Threading.Tasks.Task.Delay(50);

            var dialog = new ContentDialog
            {
                Title = titre,
                Content = contenu,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }
}
