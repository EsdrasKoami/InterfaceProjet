using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceProjet.Classes
{
    internal class Administrateur
    {
        private int idAdmin;
        private string nomUtilisateur;
        private string motDePasseHash;
        private DateTime dateCreation;
        private DateTime? derniereConnexion;

        // Constructeur principal
        public Administrateur(string nomUtilisateur, string motDePasseHash)
        {
            this.NomUtilisateur = nomUtilisateur;
            this.MotDePasseHash = motDePasseHash;
            this.DateCreation = DateTime.Now;
            this.DerniereConnexion = null;
        }

       
        // Properties
        public int IdAdmin { get => idAdmin; set => idAdmin = value; }
        public string NomUtilisateur { get => nomUtilisateur; set => nomUtilisateur = value; }
        public string MotDePasseHash { get => motDePasseHash; set => motDePasseHash = value; }
        public DateTime DateCreation { get => dateCreation; set => dateCreation = value; }
        public DateTime? DerniereConnexion { get => derniereConnexion; set => derniereConnexion = value; }

        public override string ToString()
        {
            return $"Admin: {NomUtilisateur}";
        }
    }
}
