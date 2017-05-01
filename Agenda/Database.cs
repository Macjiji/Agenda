using Android.Database.Sqlite;
using System.IO;
using System.Collections.Generic;


namespace Agenda
{
    class Database
    {

        /// <summary>
        /// Liste des attributs
        /// </summary>
        private SQLiteDatabase sqldb;
        private string sqldb_query;
        private string sqldb_message;
        private bool sqldb_available;

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        /// <example>
        ///     Méthode de mise en oeuvre :
        ///     <code>
        ///         Database bdd = new Database(); 
        ///     </code>
        /// </example>
        public Database()
        {
            sqldb_message = "";
            sqldb_available = false;
        }

        /// <summary>
        /// Constructeur prenant en paramètre le nom de la base de données
        /// </summary>
        /// <param name="sqldb_name">Le nom de la base de données sur laquelle effectuer les opération</param>
        /// <example>
        ///     Méthode de mise en oeuvre :
        ///     <code>
        ///         Database bdd = new Database("Evenements")
        ///     </code>
        /// </example>
        public Database(string sqldb_name)
        {
            try
            {
                sqldb_message = "";
                sqldb_available = false;
                CreateDatabase(sqldb_name);
            }
            catch (SQLiteException ex)
            {
                sqldb_message = ex.Message;
            }
        }

        /// <summary>
        /// Getter et Setter du booléen sqldb_available
        /// </summary>
        /// <example>
        ///     Méthode de mise en oeuvre pour le Getter :
        ///     <code>
        ///         boolean isDatabaseAvailable = Database.DatabaseAvailable
        ///     </code>
        ///     Méthode de mise en oeuvre pour le Setter :
        ///     <code>
        ///         Database.DatabaseAvailable = true;
        ///     </code>
        /// </example>
        public bool DatabaseAvailable
        {
            get { return sqldb_available; }
            set { sqldb_available = value; }
        }

        /// <summary>
        ///     Getter et Setter pour le message
        /// </summary>
        public string Message
        {
            get { return sqldb_message; }
            set { sqldb_message = value; }
        }

        /// <summary>
        ///     Méthode de création de la base de données. Cette méthode est mise en privée car elle n'est utilisée que par le constructeur Database(string dbname)
        /// </summary>
        /// <param name="sqldb_name">Le nom de la base de données à créer</param>
        /// <see cref="Database(string)"/>
        private void CreateDatabase(string sqldb_name)
        {
            try
            {
                sqldb_message = "";
                string sqldb_location = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                string sqldb_path = Path.Combine(sqldb_location, sqldb_name);
                bool sqldb_exists = File.Exists(sqldb_path);
                if (!sqldb_exists)
                {
                    sqldb = SQLiteDatabase.OpenOrCreateDatabase(sqldb_path, null);
                    sqldb_query = "CREATE TABLE IF NOT EXISTS TableEvenement (_id INTEGER PRIMARY KEY AUTOINCREMENT, Nom TEXT NOT NULL, Description TEXT DEFAULT NULL, Date LONG NOT NULL, Jour INTEGER NOT NULL, Mois INTEGER NOT NULL, Annee INTEGER NOT NULL);";
                    sqldb.ExecSQL(sqldb_query);
                    sqldb_message = "Base de données : " + sqldb_name + " a été créée avec succès !";
                }
                else
                {
                    sqldb = SQLiteDatabase.OpenDatabase(sqldb_path, null, DatabaseOpenFlags.OpenReadwrite);
                    sqldb_message = "Base de données : " + sqldb_name + " a été ouverte avec succès !";
                }
                sqldb_available = true;
            }
            catch (SQLiteException ex)
            {
                sqldb_message = ex.Message;
            }
        }

        /// <summary>
        ///     Méthode permettant d'ajouter un événement à la base de données
        /// </summary>
        /// <param name="evenement">L'événement à ajouter</param>
        /// <example>
        ///     Méthode de mise en oeuvre :
        ///     <code>
        ///         Database bdd = new Database("Evenements");
        ///         bdd.AddEvent(new Evenement(0, "Test", "Ceci est un test !", 1491211155, 3, 4, 2017));
        ///     </code>
        /// </example>
        public void AddEvent(Evenement evenement)
        {
            try
            {
                sqldb_query = "INSERT INTO TableEvenement (Nom, Description, Date, Jour, Mois, Annee) VALUES ('"
                            + evenement.Nom + "', '"
                            + evenement.Description + "', "
                            + evenement.Date + ", "
                            + evenement.Jour + ", "
                            + evenement.Mois + ", "
                            + evenement.Annee + ");";
                sqldb.ExecSQL(sqldb_query);
                sqldb_message = "Ajout de l'événement effectué !";
            }
            catch (SQLiteException ex)
            {
                sqldb_message = ex.Message;
            }
        }

        /// <summary>
        ///     Méthode permettant de mettre à jour un événement déjà présent dans la base de données.
        /// </summary>
        /// <param name="evenement">L'événement mis à jour</param>
        /// <example>
        ///     Méthode de mise en oeuvre :
        ///     <code>
        ///         Database bdd = new Database("Evenements");
        ///         bdd.updateEvent(new Evenement(0, "Test", "Ceci est mis à jours !", 1491211155, 3, 4, 2017));
        ///     </code>
        /// </example>
        public void UpdateEvent(Evenement evenement)
        {
            try
            {
                sqldb_query = "UPDATE TableEvenement SET Nom ='" + evenement.Nom
                                + "', Description ='" + evenement.Description
                                + "', Date ='" + evenement.Date
                                + "', Jour = '" + evenement.Jour
                                + "', Mois = '" + evenement.Mois
                                + "', Annee = '" + evenement.Annee
                                + "' WHERE _id ='" + evenement.Identifiant + "';";
                sqldb.ExecSQL(sqldb_query);
                sqldb_message = "Evenement " + evenement.Nom + " mise à jo";
            }
            catch (SQLiteException ex)
            {
                sqldb_message = ex.Message;
            }
        }

        /// <summary>
        ///     Méthode permettant de supprimer un événement dans notre base de données
        /// </summary>
        /// <param name="identifiant">L'identifiant de l'événement à supprimer</param>
        /// <example>
        ///     Méthode de mise en oeuvre :
        ///     <code>
        ///         Database bdd = new Database("Evenements");
        ///         bdd.DeleteEvent(1);
        ///     </code>
        /// </example>
        public void DeleteEvent(int identifiant)
        {
            try
            {
                sqldb_query = "DELETE FROM TableEvenement WHERE _id ='" + identifiant + "';";
                sqldb.ExecSQL(sqldb_query);
                sqldb_message = "Enregistrement " + identifiant + " supprimé avec succès";
            }
            catch (SQLiteException ex)
            {
                sqldb_message = ex.Message;
            }
        }

        /// <summary>
        ///     Méthode permettant de sélectionner un événement dans la base de données à partir de son nom.
        /// </summary>
        /// <param name="name">Le Nom de l'événement</param>
        /// <returns>L'événement voulu</returns>
        /// <example>
        ///     Méthode de mise en oeuvre :
        ///     <code>
        ///         Database bdd = new Database("Evenements");
        ///         Evenement evenementTest = bdd.GetEventWithName("Test");
        ///     </code>
        /// </example>
        public Evenement GetEventWithName(string name)
        {
            Evenement evenementARetourner = new Evenement();

            Android.Database.ICursor sqldb_cursor = null;
            try
            {
                sqldb_query = "SELECT * FROM TableEvenement WHERE Nom = '" + name + "';";
                sqldb_cursor = sqldb.RawQuery(sqldb_query, null);
                if (!(sqldb_cursor != null))
                {
                    sqldb_message = "L'enregistrement n'a pas été trouvé ! ";
                    evenementARetourner = null;
                    sqldb_cursor.Close();
                }
                else
                {
                    sqldb_message = "L'enregistrement a été trouvé ! ";
                    sqldb_cursor.MoveToFirst();
                    evenementARetourner.Identifiant = sqldb_cursor.GetInt(sqldb_cursor.GetColumnIndex("_id"));
                    evenementARetourner.Nom = sqldb_cursor.GetString(sqldb_cursor.GetColumnIndex("Nom"));
                    evenementARetourner.Description = sqldb_cursor.GetString(sqldb_cursor.GetColumnIndex("Description"));
                    evenementARetourner.Date = sqldb_cursor.GetLong(sqldb_cursor.GetColumnIndex("Date"));
                    evenementARetourner.Jour = sqldb_cursor.GetInt(sqldb_cursor.GetColumnIndex("Jour"));
                    evenementARetourner.Mois = sqldb_cursor.GetInt(sqldb_cursor.GetColumnIndex("Mois"));
                    evenementARetourner.Annee = sqldb_cursor.GetInt(sqldb_cursor.GetColumnIndex("Annee"));
                    sqldb_cursor.Close();
                }
            }
            catch (SQLiteException ex)
            {
                sqldb_message = ex.Message;
            }
            return evenementARetourner;

        }

        /// <summary>
        ///     Méthode permettant de récupérer la liste des événements présents dans la base de données.
        /// </summary>
        /// <returns>La liste des événements</returns>
        /// <example>
        ///     Méthode de mise en oeuvre :
        ///     <code>
        ///         Database bdd = new Database("Evenements");
        ///         ArrayList listeEvenements = bdd.GetAllEvents();
        ///     </code>
        /// </example>
        public List<Evenement> GetAllEvents()
        {
            List<Evenement> listeEvenements = new List<Evenement>();
            Android.Database.ICursor sqldb_cursor = null;
            try
            {
                sqldb_query = "SELECT * FROM TableEvenement;";
                sqldb_cursor = sqldb.RawQuery(sqldb_query, null);
                if (!(sqldb_cursor != null))
                {
                    sqldb_message = "Pas d'événements trouvés ! ";
                    listeEvenements = null;
                    sqldb_cursor.Close();
                }
                else
                {
                    sqldb_message = "Liste d'événements créée ! ";
                    sqldb_cursor.MoveToFirst();
                    for (int i = 0; i < sqldb_cursor.Count; i++)
                    {
                        Evenement evenementAAjouter = new Evenement();
                        evenementAAjouter.Identifiant = sqldb_cursor.GetInt(sqldb_cursor.GetColumnIndex("_id"));
                        evenementAAjouter.Nom = sqldb_cursor.GetString(sqldb_cursor.GetColumnIndex("Nom"));
                        evenementAAjouter.Description = sqldb_cursor.GetString(sqldb_cursor.GetColumnIndex("Description"));
                        evenementAAjouter.Date = sqldb_cursor.GetLong(sqldb_cursor.GetColumnIndex("Date"));
                        evenementAAjouter.Jour = sqldb_cursor.GetInt(sqldb_cursor.GetColumnIndex("Jour"));
                        evenementAAjouter.Mois = sqldb_cursor.GetInt(sqldb_cursor.GetColumnIndex("Mois"));
                        evenementAAjouter.Annee = sqldb_cursor.GetInt(sqldb_cursor.GetColumnIndex("Annee"));
                        listeEvenements.Add(evenementAAjouter);
                        sqldb_cursor.MoveToNext();
                    }
                    sqldb_cursor.Close();
                }
            }
            catch (SQLiteException ex)
            {
                sqldb_message = ex.Message;
            }
            return listeEvenements;
        }



    }
}