using System;

namespace InterfaceProjet.Classes
{
    public class Client
    {
        private int idClient;
        private string nom;
        private string adresse;
        private string telephone;
        private string email;
        private DateTime dateCreation;

        // Constructeur principal
        public Client(int idClient, string nom, string adresse, string telephone, string email, DateTime dateCreation)
        {
            this.idClient = idClient;
            this.nom = nom;
            this.adresse = adresse;
            this.telephone = telephone;
            this.email = email;
            this.dateCreation = dateCreation;
        }

        // Propriétés
        public int IdClient { get => idClient; set => idClient = value; }
        public string Nom { get => nom; set => nom = value; }
        public string Adresse { get => adresse; set => adresse = value; }
        public string Telephone { get => telephone; set => telephone = value; }
        public string Email { get => email; set => email = value; }
        public DateTime DateCreation { get => dateCreation; set => dateCreation = value; }

        public override string ToString()
        {
            return $"{IdClient} - {Nom}";
        }
    }
}
