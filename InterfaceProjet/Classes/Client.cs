using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceProjet.Classes
{
    internal class Client
    {
        private int idClient;
        private string nom;
        private string adresse;
        private string telephone;
        private string email;
        private DateTime dateCreation;

        // Constructeur principal
        public Client(int idClient, string nom, string adresse, string telephone, string email)
        {
            this.IdClient = idClient;
            this.Nom = nom;
            this.Adresse = adresse;
            this.Telephone = telephone;
            this.Email = email;
            this.DateCreation = DateTime.Now;
        }

       
        // Properties
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
