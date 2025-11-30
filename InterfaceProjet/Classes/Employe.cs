using System;

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

        // Constructeur
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
            this.PhotoUrl = photoUrl ?? ""; // ✅ Gérer les null
            this.Statut = statut;
            this.DateCreation = DateTime.Now;
        }

        // Properties de base
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

        // ✅ PROPRIÉTÉS FORMATÉES pour le XAML (pas des méthodes!)
        public string DateNaissanceFormatee => DateNaissance != DateTime.MinValue
            ? DateNaissance.ToString("dd/MM/yyyy")
            : "N/A";

        public string DateEmbaucheFormatee => DateEmbauche != DateTime.MinValue
            ? DateEmbauche.ToString("dd/MM/yyyy")
            : "N/A";

        public string TauxHoraireFormate => $"{TauxHoraire:F2} $/h";

        public string NomComplet => $"{Prenom} {Nom}"; // ✅ PROPRIÉTÉ, pas méthode!

        public string AgeTexte => DateNaissance != DateTime.MinValue
            ? $"{CalculerAge()} ans"
            : "N/A";

        // Méthode utile pour calculer l'âge
        public int CalculerAge()
        {
            if (DateNaissance == DateTime.MinValue) return 0;

            var today = DateTime.Today;
            var age = today.Year - DateNaissance.Year;
            if (DateNaissance.Date > today.AddYears(-age)) age--;
            return age;
        }

        public override string ToString()
        {
            return $"{Prenom} {Nom} ({Matricule}) - {Statut}";
        }
    }
}