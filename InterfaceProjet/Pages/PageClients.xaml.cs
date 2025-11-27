using InterfaceClient.Singletons;
using InterfaceProjet.Classes;
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
public sealed partial class PageClients : Page
{
    public PageClients()
    {
        InitializeComponent();
        lvClients.ItemsSource = SingletonClient.getInstance().Liste;
        SingletonClient.getInstance().getAllClients();
    }

    private void tbRechercheClient_TextChanged(object sender, TextChangedEventArgs e)
    {
       
    }

    private void btnAjouterClient_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(PageAjoutClient));
    }

    private void btnModifierClient_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnSupprimerClient_Click(object sender, RoutedEventArgs e)
    {

    }

    private void tbRechercheProjet_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {

    }
    private void btnAjouterProjet_Click(object sender, RoutedEventArgs e)
    {
        if (lvClients.SelectedItem is Client clientSelectionne)
        {
            // Ouvrir une page ou un dialogue pour ajouter un projet à ce client
            // Exemple :
            // Frame.Navigate(typeof(PageProjets), clientSelectionne);
        }
        else
        {
            var dialog = new ContentDialog
            {
                Title = "Aucun client sélectionné",
                Content = "Veuillez d’abord sélectionner un client avant d’ajouter un projet.",
                CloseButtonText = "OK"
            };
            _ = dialog.ShowAsync();
        }
    }


    

    

}
