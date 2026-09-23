using InterfaceProjet.Singletons;
using Microsoft.UI.Xaml.Controls;

namespace InterfaceProjet.Pages;

/// <summary>
/// Page d'accueil présentant le tableau de bord des projets en cours.
/// </summary>
public sealed partial class PageAccueil : Page
{
    private readonly SingletonProjet _projetsSingleton;

    public PageAccueil()
    {
        InitializeComponent();

        _projetsSingleton = SingletonProjet.getInstance();

        // Chargement initial des projets en cours
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
