using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceProjet.Classes
{
    public class Employe
    {
        private string matricule;
        private string nom;
        private string prenom;
        private DateTime dateNaissance;
        private string email;
        private string adresse;
        private DateTime dateEmbauche;
        private decimal tauxHoraire;
        private string photoUrl;
        private string statut;
        private DateTime dateCreation;



        public Employe(string matricule, string nom, string prenom, DateTime dateNaissance,
                       string email, string adresse, DateTime dateEmbauche, decimal tauxHoraire,
                       string photoUrl, string statut)
        {
            this.Matricule = matricule;
            this.Nom = nom;
            this.Prenom = prenom;
            this.DateNaissance = dateNaissance;
            this.Email = email;
            this.Adresse = adresse;
            this.DateEmbauche = dateEmbauche;
            this.TauxHoraire = tauxHoraire;
            this.PhotoUrl = photoUrl;
            this.Statut = statut;
            this.DateCreation = DateTime.Now;
        }


        // Properties
        public string Matricule { get => matricule; set => matricule = value; }
        public string Nom { get => nom; set => nom = value; }
        public string Prenom { get => prenom; set => prenom = value; }
        public DateTime DateNaissance { get => dateNaissance; set => dateNaissance = value; }
        public string Email { get => email; set => email = value; }
        public string Adresse { get => adresse; set => adresse = value; }
        public DateTime DateEmbauche { get => dateEmbauche; set => dateEmbauche = value; }
        public decimal TauxHoraire { get => tauxHoraire; set => tauxHoraire = value; }
        public string PhotoUrl { get => photoUrl; set => photoUrl = value; }
        public string Statut { get => statut; set => statut = value; }
        public DateTime DateCreation { get => dateCreation; set => dateCreation = value; }


        // Méthodes utiles
        public int CalculerAge()
        {
            var today = DateTime.Today;
            var age = today.Year - DateNaissance.Year;
            if (DateNaissance.Date > today.AddYears(-age)) age--;
            return age;
        }

        public string NomComplet()
        {
            return $"{Prenom} {Nom}";
        }

        public override string ToString()
        {
            return $"{Prenom} {Nom} ({Matricule}) - {Statut}";
        }
    }
}
