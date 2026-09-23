using System;

namespace InterfaceProjet.Classes
{
    internal class Administrateur
    {
        private int idAdmin;
        private string nomUtilisateur = string.Empty;
        private string motDePasseHash = string.Empty;
        private DateTime dateCreation;
        private DateTime? derniereConnexion;

        // Constructeur principal
        public Administrateur(string nomUtilisateur, string motDePasseHash)
        {
            this.nomUtilisateur = nomUtilisateur;
            this.motDePasseHash = motDePasseHash;
            this.dateCreation = DateTime.Now;
            this.derniereConnexion = null;
        }

        // Propriétés
        public int IdAdmin { get => idAdmin; set => idAdmin = value; }
        public string NomUtilisateur { get => nomUtilisateur; set => nomUtilisateur = value; }
        public string MotDePasseHash { get => motDePasseHash; set => motDePasseHash = value; }
        public DateTime DateCreation { get => dateCreation; set => dateCreation = value; }
        public DateTime? DerniereConnexion { get => derniereConnexion; set => derniereConnexion = value; }

        public override string ToString()
        {
            return $"Admin : {NomUtilisateur}";
        }
    }
}
