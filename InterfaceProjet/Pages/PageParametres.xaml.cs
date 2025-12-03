using InterfaceProjet.Singletons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace InterfaceProjet.Pages
{
    public sealed partial class PageParametres : Page
    {
        public PageParametres()
        {
            InitializeComponent();
        }

      
        // === Exporter les projets en CSV ===
        private async void BtnExporterProjetsCsv_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    // 1) Création du FileSavePicker (comme dans le PDF)
            //    var picker = new FileSavePicker();

            //    // IMPORTANT : utiliser la fenêtre principale App.fenetrePrincipale
            //    var hWnd = WindowNative.GetWindowHandle(App.fenetrePrincipale);
            //    InitializeWithWindow.Initialize(picker, hWnd);

            //    picker.SuggestedFileName = "projets";
            //    picker.FileTypeChoices.Add("Fichier CSV", new List<string>() { ".csv" });

            //    // 2) L'utilisateur choisit l'emplacement
            //    var fichier = await picker.PickSaveFileAsync();

            //    if (fichier == null)
            //        return; // l'utilisateur a annulé

            //    // 3) Appel du singleton pour écrire les projets dans le fichier
            //    //    On lui passe simplement le chemin complet
            //    SingletonProjet
            //        .getInstance()
            //        .ExporterProjetsCsv(fichier.Path);

            //    // 4) Petit message de confirmation
            //    var dlg = new ContentDialog
            //    {
            //        Title = "Exportation réussie",
            //        Content = $"Les projets ont été exportés dans :\n{fichier.Path}",
            //        CloseButtonText = "OK",
            //        XamlRoot = this.Content.XamlRoot
            //    };

            //    await dlg.ShowAsync();
            //}
            //catch (Exception ex)
            //{
            //    var dlgErr = new ContentDialog
            //    {
            //        Title = "Erreur d'exportation",
            //        Content = "Une erreur est survenue lors de l'exportation des projets :\n" + ex.Message,
            //        CloseButtonText = "OK",
            //        XamlRoot = this.Content.XamlRoot
            //    };

            //    await dlgErr.ShowAsync();
            //}
        }
    }
}
