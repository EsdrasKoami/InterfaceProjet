using InterfaceProjet.Classes;
using InterfaceProjet.Singletons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace InterfaceProjet.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PageProjets : Page
{
    private readonly SingletonProjet _projetsSingleton;
  
public PageProjets()
{

    InitializeComponent();
        listeProjets.ItemsSource = SingletonProjet.getInstance().Liste;
        SingletonProjet.getInstance().getAllProjets();

    }



private async void listeProjets_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        if (listeProjets.SelectedItem is Projet projet)
        {
            var dialog = new ProjetDetailsDialog(projet, new ObservableCollection<Assignation>(), null);
            dialog.XamlRoot = this.Content.XamlRoot;

            await dialog.ShowAsync();
            listeProjets.SelectedItem = null;
        }


    }

    private void tbRechercheProjet_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void btnAjouter_Click(object sender, RoutedEventArgs e)
    {

        Projet projet = listeProjets.SelectedItem as Projet;
        // Naviguer vers la page d’assignation en passant le projet sélectionné
        Frame.Navigate(typeof(PageAjoutPojet), projet);

    }

    private void tbRechercheProjet_TextChanged_1(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {

    }
    private async void assigner_Click(object sender, RoutedEventArgs e)
    {
        // Récupérer le projet sélectionné à partir du DataContext du bouton
        var recupere  = sender as FrameworkElement;
        Projet projetSelectionne = recupere?.DataContext as Projet;

        if (projetSelectionne == null)
        {
            var dlg = new ContentDialog
            {
                Title = "Erreur",
                Content = "Impossible de récupérer le projet sélectionné.",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await dlg.ShowAsync();
            return;
        }

       
        var dialog = new ContentDialog
        {
            Title = $"Projet {projetSelectionne.NumeroProjet}",
            Content = "Que souhaitez-vous faire ?",
            PrimaryButtonText = "Assigner client",
            SecondaryButtonText = "Assigner employé",
            CloseButtonText = "Annuler",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();

        switch (result)
        {
            case ContentDialogResult.Primary:
               
                if (this.Frame != null)
                {
                    Frame.Navigate(typeof(PageAssignationClient), projetSelectionne);
                }
                else
                {
                    Debug.WriteLine("Frame est null, navigation vers PageAssignationClient impossible.");
                }
                break;

            case ContentDialogResult.Secondary:
               
                if (this.Frame != null)
                {
                    Frame.Navigate(typeof(PageAssignationEmploye), projetSelectionne);
                }
                else
                {
                    Debug.WriteLine("Frame est null, navigation vers PageAssignationEmploye impossible.");
                }
                break;

            case ContentDialogResult.None:
            default:
                
                break;
        }
    }

    private async void ButtonModifier_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is Projet projet)
        {
            var dialog = new ModifierProjetDialog(projet)
            {
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
            // La GridView se mettra à jour si ta liste est celle du Singleton
        }
    }
}
