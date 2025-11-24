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
    Debug.WriteLine(">>> CONSTRUCTEUR PageProjets APPELÉ !!!");

    InitializeComponent();
        listeProjets.ItemsSource = SingletonProjet.getInstance().Liste;
        SingletonProjet.getInstance().getAllProjets();

    }



private void listeProjets_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        Projet projet = listeProjets.SelectedItem as Projet;
        Frame.Navigate(typeof(ProjetDetailsDialog), projet);

    }

    private void tbRechercheProjet_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void btnAjouter_Click(object sender, RoutedEventArgs e)
    {

    }

    private void tbRechercheProjet_TextChanged_1(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {

    }
    private void BtnAssigner_Click(object sender, RoutedEventArgs e)
    {
      
            Projet projet = listeProjets.SelectedItem as Projet;
            // Naviguer vers la page d’assignation en passant le projet sélectionné
            Frame.Navigate(typeof(PageAssignationEmploye), projet);
        
    }

}
