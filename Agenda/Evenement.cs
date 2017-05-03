

namespace Agenda
{
    class Evenement
    {

        private int identifiant;
        private string nom;
        private string description;
        private long date;
        private int jour;
        private int mois;
        private int annee;

        public Evenement() { }

        public Evenement(int identifiant, string nom, string description, long date, int jour, int mois, int annee)
        {
            this.identifiant = identifiant;
            this.nom = nom;
            this.description = description;
            this.date = date;
            this.jour = jour;
            this.mois = mois;
            this.annee = annee;
        }

        public int Identifiant
        {
            get { return this.identifiant; }
            set { this.identifiant = value; }
        }

        public string Nom
        {
            get { return this.nom; }
            set { this.nom = value; }
        }

        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        public long Date
        {
            get { return this.date; }
            set { this.date = value; }
        }

        public int Jour
        {
            get { return this.jour; }
            set { this.jour = value; }
        }

        public int Mois
        {
            get { return this.mois; }
            set { this.mois = value; }
        }

        public int Annee
        {
            get { return this.annee; }
            set { this.annee = value; }
        }

        public override string ToString()
        {
            return "Evenement { Identifiant : " + this.identifiant
                        + ", Nom : " + this.nom
                        + ", Description : " + this.description
                        + ", Date : " + this.date
                        + ", Jour : " + this.jour
                        + ", Mois : " + this.mois
                        + ", Année : " + this.annee + "}";
        }



    }
}