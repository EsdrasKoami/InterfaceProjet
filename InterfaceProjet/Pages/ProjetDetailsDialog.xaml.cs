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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace InterfaceProjet.Pages;

public sealed partial class ProjetDetailsDialog : ContentDialog
{

    public event PropertyChangedEventHandler PropertyChanged;

    private void Notify(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    public Projet Projet { get; set; }

    public Employe Employe { get; set; }
    public ObservableCollection<Assignation> Assignation { get; set; }
    public ProjetDetailsDialog(Projet projet, ObservableCollection<Assignation> assignations, Employe employe)
    {
        this.InitializeComponent();

        Projet = projet;
        Assignation = assignations;
        Employe = employe;
        this.DataContext = this;
    }

 
  
    public decimal TotalSalaires => Assignation?.Sum(a => a.SalaireAPayer) ?? 0;

   
    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int idAssignation)
        {
            // Suppression BD
            Singletons.SingletonAssignation.getInstance().SupprimerAssignation(idAssignation);

            // Suppression locale
            var assignation = Assignation.FirstOrDefault(a => a.IdAssignation == idAssignation);
            if (assignation != null)
                Assignation.Remove(assignation);

            // Mise à jour du total
            Notify(nameof(TotalSalaires));
        }
    }

}
