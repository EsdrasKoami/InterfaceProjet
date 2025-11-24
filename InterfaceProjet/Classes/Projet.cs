using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceProjet.Classes
{
    public class Projet
    {
        private string numeroProjet;
        private string titre;
        private DateTime dateDebut;
        private string description;
        private decimal budget;
        private int nbEmployesRequis;
        private decimal totalSalaires;
        private int idClient;
        private string nomClient;
        private string statut;
        private DateTime dateCreation;

        // Nouveaux champs ajoutés
        private int nbEmployesAssignes;
        private string telephoneClient;

        public Projet(string numeroProjet, string titre, DateTime dateDebut, string description, decimal budget, int nbEmployesRequis, decimal totalSalaires, int idClient, string nomClient, string statut, DateTime dateCreation)
        {
            this.numeroProjet = numeroProjet;
            this.titre = titre;
            this.dateDebut = dateDebut;
            this.description = description;
            this.budget = budget;
            this.nbEmployesRequis = nbEmployesRequis;
            this.totalSalaires = totalSalaires;
            this.idClient = idClient;
            this.nomClient = nomClient;
            this.statut = statut;
            this.dateCreation = dateCreation;
        }




        // Properties
        public string NumeroProjet { get => numeroProjet; set => numeroProjet = value; }
        public string Titre { get => titre; set => titre = value; }
        public DateTime DateDebut { get => dateDebut; set => dateDebut = value; }
        public string Description { get => description; set => description = value; }
        public decimal Budget { get => budget; set => budget = value; }
        public int NbEmployesRequis { get => nbEmployesRequis; set => nbEmployesRequis = value; }
        public decimal TotalSalaires { get => totalSalaires; set => totalSalaires = value; }
        public int IdClient { get => idClient; set => idClient = value; }
        public string Statut { get => statut; set => statut = value; }
        public DateTime DateCreation { get => dateCreation; set => dateCreation = value; }
        public string NomClient { get => nomClient; set => nomClient = value; }

        // Nouveaux champs Properties
        public int NbEmployesAssignes { get => nbEmployesAssignes; set => nbEmployesAssignes = value; }
        public string TelephoneClient { get => telephoneClient; set => telephoneClient = value; }

        // Méthodes utiles
        public bool EstEnCours() => Statut == "En cours";
        public bool EstTermine() => Statut == "Terminé";

        public decimal BudgetRestant() => Budget - TotalSalaires;

        public decimal PourcentageBudgetUtilise()
        {
            if (Budget == 0) return 0;
            return (TotalSalaires / Budget) * 100;
        }

        public override string ToString() => $"{NumeroProjet} - {Titre} ({Statut})";
    }
}
