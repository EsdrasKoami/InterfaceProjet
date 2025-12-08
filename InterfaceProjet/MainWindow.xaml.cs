using InterfaceAdmin.Singletons;
using InterfaceProjet.Classes;
using InterfaceProjet.Pages;
using InterfaceProjet.Singletons;
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
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using WinRT.Interop;

namespace InterfaceProjet
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //  CHARGER LE THÈME SAUVEGARDÉ AU DÉMARRAGE
            ChargerThemeSauvegarde();

            var maintenant = DateTime.Now;
            tbDate.Text = maintenant.ToString("dd/MM/yyyy");
            tbHeure.Text = maintenant.ToString("HH:mm");

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(titlebar);

            VerifierEtNaviguer();
        }

        //  NOUVELLE MÉTHODE : Charge le thème au démarrage
        private void ChargerThemeSauvegarde()
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            // Récupérer le thème sauvegardé (par défaut "Light")
            string themeSauvegarde = localSettings.Values["AppTheme"] as string ?? "Dark";

            // Appliquer le thème
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
            {
                mainFrame.Navigate(typeof(PageAdmin));
            }
            else
            {
                ActiverNavigation();
            }
        }

        public void ActiverNavigation()
        {
            mainFrame.Navigate(typeof(PageAccueil));
        }

        private async void navView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
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

            if (monFichier != null)
            {
                await FileIO.WriteLinesAsync(
                    monFichier,
                    lignes,
                    Windows.Storage.Streams.UnicodeEncoding.Utf8
                );
            }

            var dialog = new ContentDialog
            {
                Title = "Exportation réussie",
                Content = $"Les projets ont bien été exportés dans :\n{monFichier.Path}",
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private async System.Threading.Tasks.Task GererDeconnexion()
        {
            if (!SingletonAdmin.getInstance().EstConnecte())
            {
                var dlg = new ContentDialog
                {
                    Title = "Aucune connexion active",
                    Content = "Vous n'êtes pas connecté en tant qu'administrateur.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dlg.ShowAsync();
                return;
            }

            var admin = SingletonAdmin.getInstance().AdministrateurConnecte;
            var confirm = new ContentDialog
            {
                Title = "Confirmation de déconnexion",
                Content = $"Voulez-vous vraiment vous déconnecter en tant que {admin.NomUtilisateur} ?",
                PrimaryButtonText = "Déconnexion",
                CloseButtonText = "Annuler",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.Content.XamlRoot
            };

            var result = await confirm.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                SingletonAdmin.getInstance().Deconnecter();

                var succes = new ContentDialog
                {
                    Title = "Déconnexion réussie",
                    Content = "Vous avez été déconnecté avec succès.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await succes.ShowAsync();

                mainFrame.Navigate(typeof(PageAccueil));
            }
        }
    }
}