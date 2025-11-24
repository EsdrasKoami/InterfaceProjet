using InterfaceProjet.Pages;
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
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace InterfaceProjet
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
  //        this.ExtendsContentIntoTitleBar = true; // Extend the content into the title bar and hide the default titlebar
  //this.SetTitleBar(titleBar);
        public MainWindow()
        {
            InitializeComponent();
            mainFrame.Navigate(typeof(PageAccueil));
            var maintenant = DateTime.Now;

            tbDate.Text = maintenant.ToString("dd/MM/yyyy"); // ou "yyyy-MM-dd"
            tbHeure.Text = maintenant.ToString("HH:mm");
            this.ExtendsContentIntoTitleBar = true; // Extend the content into the title bar and hide the default titlebar
            this.SetTitleBar(titlebar); // Set the custom title bar
        }

        private void navView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
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
                    default:
                        mainFrame.Navigate(typeof(PageAccueil));
                        break;
                }
            }

        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
