using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Widget;
using MaterialCalendarLibrary;
using System.Collections.Generic;

namespace Agenda
{

    [Activity(Label = "Agenda", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IOnDateSelectedListener, IDayViewDecorator
    {
        private MaterialCalendarView calendrier;
        private Database database;
        private List<Evenement> listeEvenements = new List<Evenement>();
        private List<CalendarDay> listeJourCalendrier = new List<CalendarDay>();

        protected ImageButton ajouterEvement;
        protected ImageButton modifierEvenement, supprimerEvenement;
        protected LinearLayout apercuEvenement;
        protected TextView nomEvenement, dateEvement, descriptionEvenement;

        /// <summary>
        ///     Méthode OnCreate nécessaire pour créer l'activité
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            InitialiserDatabase();
            InitialiserCalendar();
            InitialiserButtons();
            InitialiserEventLayout();
        }

        /// <summary>
        ///     Méthode permettant d'initialiser la base de données.
        /// </summary>
        private void InitialiserDatabase()
        {
            database = new Database("Evenements");
        }

        /// <summary>
        ///     Méthode permettant d'initialiser le calendrier de notre activité Main
        /// </summary>
        private void InitialiserCalendar()
        {
            // Etape 1 : On commence par initialiser le calendrier à partir de sa vue dans le fichier .axml
            calendrier = FindViewById<MaterialCalendarView>(Resource.Id.calendarView);
            calendrier.SelectionMode = MaterialCalendarView.SelectionModeSingle;
            calendrier.SetOnDateChangedListener(this);

            // Etape 2 : On récupère le contenu de la base de données
            listeEvenements = database.GetAllEvents();

            // Etape 3 : On crée une liste de jour de calendrier qui correspond aux événements présents dans la liste
            foreach(Evenement evenement in listeEvenements)
            {
                listeJourCalendrier.Add(new CalendarDay(evenement.Annee, evenement.Mois, evenement.Jour));
            }

            // Etape 4 : On ajoute les décorations des cases où il existe des événements. 
            //           En gros, cette ligne permet d'appeler les méthodes "SouldDecorate" et "Decorate" !
            //           Ici, on a d'ailleurs pas besoin de créer une classe perso pour customisé la décoration, d'où l'appel à "this" (Relatif à l'implémentation de "IDayViewDecorator")
            calendrier.AddDecorator(this);

        }

        /// <summary>
        ///     Méthode permettant d'initialiser les différents boutons de notre activité Main
        /// </summary>
        private void InitialiserButtons()
        {
            // Initialisation des boutons à partir de leurs vues
            ajouterEvement = FindViewById<ImageButton>(Resource.Id.buttonAppuie);
            modifierEvenement = FindViewById<ImageButton>(Resource.Id.modifierEvenement);
            supprimerEvenement = FindViewById<ImageButton>(Resource.Id.supprimerEvenement);

            // Gestion du clic sur le bouton d'ajout d'un événement
            ajouterEvement.Click += delegate { StartActivity(typeof(AjouterEvenement)); };

            // Gestion du clic sur le bouton de modification d'un événement
            modifierEvenement.Click += delegate
            {
                Intent intentModification = new Intent(this, typeof(ModifierEvenement));
                intentModification.PutExtra("nom", nomEvenement.Text.ToString());
                StartActivity(intentModification);
            };

            // Gestion du clic sur le bouton de suppression d'un événement
            supprimerEvenement.Click += delegate
            {
                // Création de la fenêtre de dialogue
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Suppression d'un événement");
                alert.SetMessage("Voulez-vous réellement supprimer l'événement : " + nomEvenement.Text.ToString());
                alert.SetPositiveButton("Oui", (senderAlert, args) => {

                    // Etape 0 : On crée l'événement qui sera supprimer, et on lui attribue la valeur null.
                    Evenement evenementASupprimer = null;
                    CalendarDay dateASupprimer = null;

                    // Etape 1 : On parcours la liste des événements et on supprimer l'événement adéquat
                    foreach (Evenement evenement in listeEvenements)
                    {
                        if (evenement.Nom.Equals(nomEvenement.Text.ToString())){
                            database.DeleteEvent(evenement.Identifiant);
                            evenementASupprimer = evenement;
                            dateASupprimer = new CalendarDay(evenement.Annee, evenement.Mois, evenement.Jour);
                        }
                    }

                    // Etape 2 : S'il existe bien un événement à supprimer, on rafraichit l'activité
                    if(evenementASupprimer != null)
                    {
                        // Sous-étape 1 : On fait disparaitre l'aperçu de l'événement supprimé
                        apercuEvenement.Visibility = Android.Views.ViewStates.Gone;

                        // Sous-étape 2 : On enlève de liste l'événement supprimer
                        listeEvenements.Remove(evenementASupprimer);
                        listeJourCalendrier.Remove(dateASupprimer);

                        // Sous-étape 3 : On rafraichit le calendrier
                        calendrier.RemoveDecorators();
                        calendrier.InvalidateDecorators();

                    }
                });

                alert.SetNegativeButton("Non", (senderAlert, args) => { });
                Dialog dialog = alert.Create();
                dialog.Show();
            };

        }

        /// <summary>
        ///     Méthode permettant d'initialiser l'aperçu d'un événement dans notre activité Main
        /// </summary>
        private void InitialiserEventLayout()
        {
            // On initialise l'aperçu d'un événement dans une date et on le met par défaut à GONE (permet de libérer la place sur la vue).
            apercuEvenement = FindViewById<LinearLayout>(Resource.Id.apercuEvenement);
            nomEvenement = FindViewById<TextView>(Resource.Id.nomEvenement);
            dateEvement = FindViewById<TextView>(Resource.Id.dateEvenement);
            descriptionEvenement = FindViewById<TextView>(Resource.Id.descriptionEvenement);
            apercuEvenement.Visibility = Android.Views.ViewStates.Gone;
        }

        /// <summary>
        ///     Méthode issue de l'interface IOnDateSelectedListener, permettant de connaître la date sur laquelle l'utilisateur clique.
        /// </summary>
        /// <param name="p0">Le calendrier</param>
        /// <param name="p1">Le jour du calendrier cliqué</param>
        /// <param name="p2"></param>
        /// <see cref="IOnDateSelectedListener"/>
        public void OnDateSelected(MaterialCalendarView p0, CalendarDay p1, bool p2)
        {

            // Etape 1 : On crée un booléen qui va nous permettre d'afficher ou non l'aperçu d'un événement
            bool existenceEvenement = false;

            // Etape 2 : On parcours la liste des événements puis on teste*
            if (listeEvenements != null)
            {
                foreach (Evenement evenement in listeEvenements)
                {
                    if (p1.Day == evenement.Jour && p1.Month == evenement.Mois && p1.Year == evenement.Annee)
                    {
                        nomEvenement.Text = evenement.Nom;
                        dateEvement.Text = evenement.Jour + "/" + (evenement.Mois + 1) + "/" + evenement.Annee;
                        if (evenement.Description != null)
                        {
                            descriptionEvenement.Text = evenement.Description;
                        }
                        else
                        {
                            descriptionEvenement.Text = "Pas de description";
                        }
                        existenceEvenement = true;
                    }
                }
            }

            // Etape 3 : On finit par afficher ou non l'aperçu d'un événement s'il existe à une date précise
            if (existenceEvenement)
            {
                apercuEvenement.Visibility = Android.Views.ViewStates.Visible;
            }
            else
            {
                apercuEvenement.Visibility = Android.Views.ViewStates.Gone;
            }

        }

        /// <summary>
        ///     Méthode issue de l'interface IDayViewDecorator permettant de savoir si une date du calendrier doit être décorée.
        /// </summary>
        /// <param name="p0"></param>
        /// <returns>True si la date existe dans la liste des événements, False sinon</returns>
        /// <see cref="IDayViewDecorator"/>
        public bool ShouldDecorate(CalendarDay p0)
        {
            // On crée un booléen pour vérifier si la date existe dans la liste des événements dans la base de données
            // On met par défaut le booléen à False, comme ça, si la liste des jours ne contient pas la date parcourue par ShouldDecorate, on a pas besoin de changer la valeur de ce booléen.
            bool verif = false;

            // Si la liste est différente de "null" (Ne contiendrait aucun événement) :
            //      -> On parcours les jours présents dans la liste des jours du calendrier;
            //      -> Et si le jour parcours par "ShouldDecorate" correspond à une date dans la liste des jours :
            //          -> On met le booléen à True
            if(listeEvenements != null)
            {
                foreach(CalendarDay jourCalendrier in listeJourCalendrier)
                {
                    if (p0.Day == jourCalendrier.Day && p0.Month == jourCalendrier.Month && p0.Year == jourCalendrier.Year)
                    {
                        verif = true;
                    }
                }
            }

            // Enfin, on retourne le booléen. Celui ci va être "interprété" par la méthode Decorate
            return verif;
           
        }

        /// <summary>
        ///     Méthode permettant de décorer l'arrière plan d'une case du calendrier. Elle est dessiner que si la méthode ShouldDecorate renvoie True !
        /// </summary>
        /// <param name="p0"></param>
        /// <see cref="IDayViewDecorator"/>
        /// <see cref="ShouldDecorate(CalendarDay)"/>
        public void Decorate(DayViewFacade p0)
        {
            // En gros, il faut comprendre ici que si ShouldDecorate est True, le calendrier décorera cette case.
            p0.SetBackgroundDrawable(new ColorDrawable(Color.Rgb(37, 155, 36)));
        }


    }


}

