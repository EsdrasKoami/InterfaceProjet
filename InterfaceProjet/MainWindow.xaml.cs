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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace InterfaceProjet
{
    /// <summary>
    // An empty window that can be used on its own or navigated to within a Frame.
  
    public sealed partial class MainWindow : Window
    {
      
        public MainWindow()
        {
            InitializeComponent();
     
            
            var maintenant = DateTime.Now;

            tbDate.Text = maintenant.ToString("dd/MM/yyyy");
            tbHeure.Text = maintenant.ToString("HH:mm");
            this.ExtendsContentIntoTitleBar = true; 
            this.SetTitleBar(titlebar);
            VerifierEtNaviguer();
        }
        private void VerifierEtNaviguer()
        {
            bool adminExiste = SingletonAdmin.getInstance().AdministrateurExiste();

            if (!adminExiste)
            {
                // Pas d'admin ? Afficher seulement la PageAdmin dans mainFrame
              
                mainFrame.Navigate(typeof(PageAdmin));
            }
            else
            {
                // Admin existe ? Afficher le NavigationView + PageAccueil
                ActiverNavigation();
            }
        }

        public void ActiverNavigation()
        {
           
            mainFrame.Navigate(typeof(PageAccueil));
        }

        //  MÉTHODE APPELÉE PAR LE XAML
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
            // 1) Préparer le FileSavePicker (comme en classe)
            var picker = new Windows.Storage.Pickers.FileSavePicker();
            var hWnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(picker, hWnd);

            picker.SuggestedFileName = "projets";
            picker.FileTypeChoices.Add("Fichier CSV", new List<string>() { ".csv" });

            // 2) Choix du fichier
            StorageFile monFichier = await picker.PickSaveFileAsync();
            if (monFichier == null)
                return; // user a annulé

            // 3) Charger les projets via le singleton
            var singleton = SingletonProjet.getInstance();
            singleton.getAllProjets();                 // va chercher en BD
            List<Projet> liste = singleton.Liste.ToList(); // On a une List<Projet>

            // 4) Construire les lignes : en-tête + ToString() de chaque projet
            var lignes = new List<string>();

            // En-tête CSV (colonnes distinctes)
            lignes.Add("NumeroProjet;Titre;NomClient;DateDebut;Budget;TotalSalaires;Statut");

            // Corps : chaque projet utilise ToString()
            lignes.AddRange(liste.ConvertAll(p => p.ToString()));

            // 5) Écriture dans le fichier CSV (comme l’exemple du prof)
            if (monFichier != null)
            {
                await FileIO.WriteLinesAsync(
                    monFichier,
                    lignes,
                    Windows.Storage.Streams.UnicodeEncoding.Utf8
                );
            }

            // 6) Message de confirmation
            var dialog = new ContentDialog
            {
                Title = "Exportation réussie",
                Content = $"Les projets ont bien été exportés dans :\n{monFichier.Path}",
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Primary,
                // adapte si tu es dans une Page :
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private async System.Threading.Tasks.Task GererDeconnexion()
        {
            // Vérifier si un admin est connecté
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

            // Demander confirmation
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
                // Déconnecter l'administrateur
                SingletonAdmin.getInstance().Deconnecter();

                // Message de confirmation
                var succes = new ContentDialog
                {
                    Title = "Déconnexion réussie",
                    Content = "Vous avez été déconnecté avec succès.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await succes.ShowAsync();

                // Rediriger vers la page d'accueil
                mainFrame.Navigate(typeof(PageAccueil));
            }
        }

    }
}
