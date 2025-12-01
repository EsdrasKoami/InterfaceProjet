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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace InterfaceProjet.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PageAccueil : Page
{
    private readonly SingletonProjet _projetsSingleton;

    public PageAccueil()
    {
        InitializeComponent();

        _projetsSingleton = SingletonProjet.getInstance();

        // Chargement initial
        _projetsSingleton.getProjetsEnCours();
        listeProjetsEnCours.ItemsSource = _projetsSingleton.Liste;
    }

    private void tbRechercheProjet_TextChanged_1(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        string motCle = sender.Text.Trim();

        if (string.IsNullOrWhiteSpace(motCle))
            _projetsSingleton.getProjetsEnCours();
        else
            _projetsSingleton.rechercherProjets(motCle);

        listeProjetsEnCours.ItemsSource = _projetsSingleton.Liste;
    }

    private void listeProjetsEnCours_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}
