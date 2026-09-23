using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using InterfaceProjet.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace InterfaceProjet
{
    /// <summary>
    /// Fournit le comportement spécifique à l'application pour compléter la classe Application par défaut.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;
        public static Window? fenetrePrincipale;

        /// <summary>
        /// Initialise l'objet d'application singleton. Il s'agit de la première ligne de code créé
        /// exécuté, et correspond logiquement à main() ou WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            this.UnhandledException += (sender, e) =>
            {
                e.Handled = true;
                try
                {
                    string path = System.IO.Path.Combine(AppContext.BaseDirectory, "error_unhandled.txt");
                    File.WriteAllText(path, $"ExceptionNonGeree : {e.Message}\n{e.Exception?.ToString()}");
                }
                catch { }
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                try
                {
                    var ex = e.ExceptionObject as Exception;
                    string path = System.IO.Path.Combine(AppContext.BaseDirectory, "error_domain.txt");
                    File.WriteAllText(path, $"ExceptionDomaine : {ex?.ToString()}");
                }
                catch { }
            };
        }

        /// <summary>
        /// Invoqué lorsque l'application est lancée normalement par l'utilisateur final.
        /// </summary>
        /// <param name="args">Détails concernant la requête de lancement et le processus.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Initialisation portable de la base de données SQLite
            DatabaseHelper.InitializeDatabase();

            _window = new MainWindow();
            fenetrePrincipale = _window;
            _window.Activate();
        }
    }
}
