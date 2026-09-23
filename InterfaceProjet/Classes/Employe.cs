using System;

namespace InterfaceProjet.Classes
{
    public class Employe
    {
        private string matricule = string.Empty;
        private string nom = string.Empty;
        private string prenom = string.Empty;
        private DateTime dateNaissance;
        private string email = string.Empty;
        private string adresse = string.Empty;
        private DateTime dateEmbauche;
        private decimal tauxHoraire;
        private string photoUrl = string.Empty;
        private string statut = string.Empty;
        private DateTime dateCreation;

        // Constructeur
        public Employe(string matricule, string nom, string prenom, DateTime dateNaissance,
                       string email, string adresse, DateTime dateEmbauche, decimal tauxHoraire,
                       string photoUrl, string statut)
        {
            this.matricule = matricule;
            this.nom = nom;
            this.prenom = prenom;
            this.dateNaissance = dateNaissance;
            this.email = email;
            this.adresse = adresse;
            this.dateEmbauche = dateEmbauche;
            this.tauxHoraire = tauxHoraire;
            this.photoUrl = photoUrl ?? string.Empty; 
            this.statut = statut;
            this.dateCreation = DateTime.Now;
        }

        // Propriétés de base
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

        // Propriétés formatées pour la liaison de données XAML
        public string DateNaissanceFormatee => DateNaissance != DateTime.MinValue
            ? DateNaissance.ToString("dd/MM/yyyy")
            : "N/A";

        public string DateEmbaucheFormatee => DateEmbauche != DateTime.MinValue
            ? DateEmbauche.ToString("dd/MM/yyyy")
            : "N/A";

        public string TauxHoraireFormate => $"{TauxHoraire:F2} $/h";

        public string NomComplet => $"{Prenom} {Nom}"; 
        public string AgeTexte => DateNaissance != DateTime.MinValue
            ? $"{CalculerAge()} ans"
            : "N/A";

        // Méthode de calcul de l'âge
        public int CalculerAge()
        {
            if (DateNaissance == DateTime.MinValue) return 0;

            var aujourdHui = DateTime.Today;
            var age = aujourdHui.Year - DateNaissance.Year;
            if (DateNaissance.Date > aujourdHui.AddYears(-age)) age--;
            return age;
        }

        public override string ToString()
        {
            return $"{Prenom} {Nom} ({Matricule}) - {Statut}";
        }
    }
}