using System;

using Android.App;
using Android.OS;
using Android.Widget;

namespace Agenda
{
    [Activity(Label = "AjouterEvenement")]
    public class AjouterEvenement : Activity, DatePickerDialog.IOnDateSetListener
    {

        private Database baseDeDonnees;
        private Evenement evenement = new Evenement();

        private EditText nomEvenement;
        private EditText descriptionEvenement;
        private EditText dateEvenement;
        private ImageButton boutonDate;
        private Button boutonValider;

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime Today = DateTime.Today;
        private DateTime date = Today;

        /// <summary>
        ///     Méthode OnCreate nécessaire pour créer l'activité
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AjouterEvenement);

            InitialiseBaseDeDonnees();
            InitialiseBoutons();
            InitialiseEditText();
        }

        /// <summary>
        ///     Méthode permettant d'initialiser la base de données.
        /// </summary>
        private void InitialiseBaseDeDonnees()
        {
            baseDeDonnees = new Database("Evenements");
        }

        /// <summary>
        ///     Méthode permettant d'initialiser les boutons. Elle contient :
        ///         -> L'initialisation du bouton d'ouverture du fragment de date;
        ///         <see cref="AfficherDatePicker"/>
        ///         -> L'initialisation du bouton de validation du formulaire.
        ///         <see cref="EvenementEstValide"/>
        /// </summary>
        private void InitialiseBoutons()
        {
            boutonValider = FindViewById<Button>(Resource.Id.buttonValider);
            boutonDate = FindViewById<ImageButton>(Resource.Id.buttonDate);

            boutonDate.Click += delegate { AfficherDatePicker(); };
            boutonValider.Click += delegate
            {
                if (EvenementEstValide())
                {
                    baseDeDonnees.AddEvent(evenement);
                    StartActivity(typeof(MainActivity));
                }
            };

        }

        /// <summary>
        ///     Méthode permettant d'initialise les champ d'édition. Elle contient :
        ///         -> L'initialisation du champ Nom de l'événement;
        ///         -> L'initialisation du champ Description de l'événement;
        ///         -> L'initialisation du champ Date de l'événement.
        /// </summary>
        private void InitialiseEditText()
        {
            nomEvenement = FindViewById<EditText>(Resource.Id.champNom);
            descriptionEvenement = FindViewById<EditText>(Resource.Id.champDescription);
            dateEvenement = FindViewById<EditText>(Resource.Id.champDate);

            nomEvenement.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => {
                evenement.Nom = e.Text.ToString();
            };

            descriptionEvenement.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                evenement.Description = e.Text.ToString();
            };
        }

        /// <summary>
        ///     Méthode permettant d'afficher le DatePicker
        /// </summary>
        private void AfficherDatePicker()
        {
            var dialog = new DatePickerFragment(this, DateTime.Now, this);
            dialog.Show(FragmentManager, null);
        }

        /// <summary>
        ///     Méthode permettant de vérifier qu'un événement est valide pour l'insérer en base de données. On va tester :
        ///         -> Si le nom de l'événement est renseigné;
        ///         -> Si la date est supérieur à la date du jour.
        /// </summary>
        /// <returns>True si l'événement est valide, False sinon</returns>
        private bool EvenementEstValide()
        {

            bool verification = true; // On considére que la vérification est valide par défaut

            if (string.IsNullOrEmpty(evenement.Nom) || string.IsNullOrWhiteSpace(evenement.Nom))
            {
                verification = false;
                nomEvenement.Error = "Vous devez renseigner un nom d'événement";
            }
            else
            {
                nomEvenement.Error = null;
            }

            if (date < Today) // On teste si la date est supérieur au jour actuel
            {
                verification = false;
                dateEvenement.Error = "La date doit être supérieur à la date actuelle !";
            }
            else
            {
                dateEvenement.Error = null;
            }

            return verification; // On retourne la valeur de vérification

        }

        /// <summary>
        ///     Méthode permettant de récupérer les informations dans le Fragment DatePicker en vue d'attribuer les valeurs à l'événement. C'est un écouteur d'événement !
        /// </summary>
        /// <param name="view">La vue du DatePicker</param>
        /// <param name="year">L'année récupérée</param>
        /// <param name="monthOfYear">Le mois récupéré</param>
        /// <param name="dayOfMonth">Le jour du mois récupéré</param>
        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            date = new DateTime(year, monthOfYear + 1, dayOfMonth);
            DateTime utc = date.ToUniversalTime();

            dateEvenement.Text = date.ToShortDateString().ToString();
            evenement.Date = (long)(utc - UnixEpoch).TotalMilliseconds / 1000L;
            evenement.Jour = dayOfMonth;
            evenement.Mois = monthOfYear;
            evenement.Annee = year;
        }

    }
}