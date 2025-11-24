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


        // Constructeur complet (pour lecture depuis BD)
        public Projet(string numeroProjet, string titre, DateTime dateDebut, string description,
                      decimal budget, int nbEmployesRequis, decimal totalSalaires, int idClient,
                      string statut, DateTime dateCreation, string nomClient)
        {
            this.NumeroProjet = numeroProjet;
            this.Titre = titre;
            this.DateDebut = dateDebut;
            this.Description = description;
            this.Budget = budget;
            this.NbEmployesRequis = nbEmployesRequis;
            this.TotalSalaires = totalSalaires;
            this.IdClient = idClient;
            this.Statut = statut;
            this.DateCreation = dateCreation;
            this.NomClient = nomClient;
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

        // Méthodes utiles
        public bool EstEnCours()
        {
            return Statut == "En cours";
        }

        public bool EstTermine()
        {
            return Statut == "Terminé";
        }

        public decimal BudgetRestant()
        {
            return Budget - TotalSalaires;
        }

        public decimal PourcentageBudgetUtilise()
        {
            if (Budget == 0) return 0;
            return (TotalSalaires / Budget) * 100;
        }

        public override string ToString()
        {
            return $"{NumeroProjet} - {Titre} ({Statut})";
        }
    }
}
