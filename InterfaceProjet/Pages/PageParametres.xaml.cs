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

namespace InterfaceProjet.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageParametres : Page
    {
        public PageParametres()
        {
            InitializeComponent();
        }
        private void BtnChangerMotDePasse_Click(object sender, RoutedEventArgs e)
        {
            // TODO : ouvrir une ContentDialog pour changer le mot de passe
            // (ancien mot de passe, nouveau, confirmation, etc.)
        }

        private void cbLangue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO : sauvegarder le choix de langue dans ta config
            // (pour l'instant tu peux juste afficher un message ou log)
        }

        private void cbFormatDateHeure_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO : sauvegarder le format choisi (affichage des dates dans l'app)
        }
    }
}
