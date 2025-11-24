using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceProjet.Classes
{
    public class Assignation
    {
        private int idAssignation;
        private string matriculeEmploye;
        private string numeroProjet;
        private decimal heuresTravaillees;
        private decimal salaireAPayer;
        private DateTime dateAssignation;
        private Employe employe;

        // Constructeur principal
        public Assignation(string matriculeEmploye, string numeroProjet,
                           decimal heuresTravaillees, decimal salaireAPayer, Employe employe)
        {
            this.MatriculeEmploye = matriculeEmploye;
            this.NumeroProjet = numeroProjet;
            this.HeuresTravaillees = heuresTravaillees;
            this.SalaireAPayer = salaireAPayer;
            this.DateAssignation = DateTime.Now;
            Employe = employe;
        }



        // Properties
        public int IdAssignation { get => idAssignation; set => idAssignation = value; }
        public string MatriculeEmploye { get => matriculeEmploye; set => matriculeEmploye = value; }
        public string NumeroProjet { get => numeroProjet; set => numeroProjet = value; }
        public decimal HeuresTravaillees { get => heuresTravaillees; set => heuresTravaillees = value; }
        public decimal SalaireAPayer { get => salaireAPayer; set => salaireAPayer = value; }
        public DateTime DateAssignation { get => dateAssignation; set => dateAssignation = value; }
        public Employe Employe { get => employe; set => employe = value; }

        // Méthodes utiles
        public decimal TauxHoraireCalcule()
        {
            if (HeuresTravaillees == 0) return 0;
            return SalaireAPayer / HeuresTravaillees;
        }

        public override string ToString()
        {
            return $"Assignation #{IdAssignation}: {MatriculeEmploye} -> {NumeroProjet} " +
                   $"({HeuresTravaillees}h = {SalaireAPayer:C})";
        }
    }
}
