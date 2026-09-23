using System;

namespace InterfaceProjet.Classes
{
    public class Projet
    {
        private string numeroProjet = string.Empty;
        private string titre = string.Empty;
        private DateTime dateDebut;
        private string description = string.Empty;
        private decimal budget;
        private int nbEmployesRequis;
        private decimal totalSalaires;
        private int? idClient;
        private string nomClient = string.Empty;
        private string statut = string.Empty;
        private DateTime dateCreation;

        private int nbEmployesAssignes;
        private string telephoneClient = string.Empty;

        // Constructeur avec idClient nullable
        public Projet(string numeroProjet, string titre, DateTime dateDebut, string description,
                      decimal budget, int nbEmployesRequis, decimal totalSalaires,
                      int? idClient, string nomClient, string statut, DateTime dateCreation)
        {
            this.numeroProjet = numeroProjet;
            this.titre = titre;
            this.dateDebut = dateDebut;
            this.description = description;
            this.budget = budget;
            this.nbEmployesRequis = nbEmployesRequis;
            this.totalSalaires = totalSalaires;
            this.idClient = idClient;
            this.nomClient = nomClient ?? "Aucun client";
            this.statut = statut;
            this.dateCreation = dateCreation;
            this.telephoneClient = string.Empty;
        }

        // Propriétés
        public string NumeroProjet { get => numeroProjet; set => numeroProjet = value; }
        public string Titre { get => titre; set => titre = value; }
        public DateTime DateDebut { get => dateDebut; set => dateDebut = value; }
        public string Description { get => description; set => description = value; }
        public decimal Budget { get => budget; set => budget = value; }
        public int NbEmployesRequis { get => nbEmployesRequis; set => nbEmployesRequis = value; }
        public decimal TotalSalaires { get => totalSalaires; set => totalSalaires = value; }
        public int? IdClient { get => idClient; set => idClient = value; }
        public string Statut { get => statut; set => statut = value; }
        public DateTime DateCreation { get => dateCreation; set => dateCreation = value; }
        public string NomClient { get => nomClient; set => nomClient = value; }

        // Propriétés additionnelles d'assignation
        public int NbEmployesAssignes { get => nbEmployesAssignes; set => nbEmployesAssignes = value; }
        public string TelephoneClient { get => telephoneClient; set => telephoneClient = value; }

        // Méthodes utiles
        public bool EstEnCours() => Statut == "En cours";
        public bool EstTermine() => Statut == "Terminé";
        public string BudgetAfficher => $"{budget}$";
        public decimal BudgetRestant() => Budget - TotalSalaires;

        public string DateDebutFormater => DateDebut != DateTime.MinValue
           ? DateDebut.ToString("dd/MM/yyyy")
           : "N/A";

        public decimal PourcentageBudgetUtilise()
        {
            if (Budget == 0) return 0;
            return (TotalSalaires / Budget) * 100;
        }

        public bool AUnClient() => IdClient.HasValue && IdClient.Value > 0;

        public override string ToString() => $"{NumeroProjet};" +
           $"{Titre};" +
           $"{NomClient};" +
           $"{DateDebutFormater};" +
           $"{Budget};" +
           $"{TotalSalaires};" +
           $"{Statut}";
    }
}
