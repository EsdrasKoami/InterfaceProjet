using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceProjet.Classes
{
    internal class Assignation
    {
        int idAssignation;
        string MatriculeEmploye;
         string numeroProjet;
        decimal heureTravaillee;
        decimal salaireApayer;
        DateTime dateAssignation;

        public Assignation(int idAssignation, string matriculeEmploye, string numeroProjet, decimal heureTravaillee, decimal salaireApayer, DateTime dateAssignation)
        {
            this.idAssignation = idAssignation;
            MatriculeEmploye = matriculeEmploye;
            this.numeroProjet = numeroProjet;
            this.heureTravaillee = heureTravaillee;
            this.salaireApayer = salaireApayer;
            this.dateAssignation = dateAssignation;
        }

        public int IdAssignation { get => idAssignation; set => idAssignation = value; }
        public string MatriculeEmploye1 { get => MatriculeEmploye; set => MatriculeEmploye = value; }
        public string NumeroProjet { get => numeroProjet; set => numeroProjet = value; }
        public decimal HeureTravaillee { get => heureTravaillee; set => heureTravaillee = value; }
        public decimal SalaireApayer { get => salaireApayer; set => salaireApayer = value; }
        public DateTime DateAssignation { get => dateAssignation; set => dateAssignation = value; }

        public override string? ToString()
        {
            return base.ToString();
        }
    }
}
