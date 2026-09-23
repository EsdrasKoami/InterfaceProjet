using InterfaceAdmin.Singletons;
using InterfaceProjet.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace InterfaceProjet.Pages
{
    public sealed partial class ProjetDetailsDialog : ContentDialog, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void Notify(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public Projet? Projet { get; set; }
        public Employe? Employe { get; set; }
        public ObservableCollection<Assignation> Assignation { get; set; }

        public ProjetDetailsDialog(Projet projet, ObservableCollection<Assignation> assignations, Employe? employe = null)
        {
            this.InitializeComponent();
            Projet = projet;
            Assignation = assignations ?? new ObservableCollection<Assignation>();
            Employe = employe;
            this.DataContext = this;

            // S'abonner aux changements de la collection
            Assignation.CollectionChanged += Assignation_CollectionChanged;

            // Vérifier si l'utilisateur est administrateur
            bool estAdmin = SingletonAdmin.getInstance().EstConnecte();

            // Choisir le modèle approprié selon le statut d'administrateur
            if (estAdmin)
            {
                lvAssignations.ItemTemplate = (DataTemplate)this.Resources["AssignationTemplateAdmin"];
            }
            else
            {
                lvAssignations.ItemTemplate = (DataTemplate)this.Resources["AssignationTemplateUser"];
            }
        }

        private void Assignation_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // Notifier le changement du total des salaires
            Notify(nameof(TotalSalaires));
        }

        public decimal TotalSalaires => Assignation?.Sum(a => a.SalaireAPayer) ?? 0;

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Fermeture de la boîte de dialogue
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int idAssignation)
            {
                // Suppression dans la couche métier
                Singletons.SingletonAssignation.getInstance().SupprimerAssignation(idAssignation);

                // Suppression locale (déclenche CollectionChanged)
                var assignation = Assignation.FirstOrDefault(a => a.IdAssignation == idAssignation);
                if (assignation != null)
                    Assignation.Remove(assignation);
            }
        }
    }
}
