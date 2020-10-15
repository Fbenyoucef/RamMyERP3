using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RamMyERP3.Models
{
    public class Ram
    {
        public int ID { get; set; }

        [Display(Name = "Collaborateur")]

        public int collaborateurID { get; set; }
        [Display(Name = "Mois")]
        public int MOIS { get; set; }


        [Display(Name = "Année")]
        public int ANNEE { get; set; }
        [Display(Name = "Signature")]
        public string SIGNATURE { get; set; }

        [Display(Name = "Date de signature")]

        public DateTime DATE_SIGNATURE { get; set; }

        [Display(Name = "Jours travaillés")]

        public double? JOURS_TRAVAILLES { get; set; }

        [Display(Name = "Jours d'absence")]

        public double? JOURS_ABSENCE { get; set; }

        [Display(Name = "Commentaire")]
        public string COMMENTAIRE { get; set; }
        public Collaborateur collaborateur { get; set; }
        public ICollection<RamDetailsAbsence> ListeRamDetailsAbsence { get; set; }
        public ICollection<RamDetailsPresence> ListeRamDetailsPresence { get; set; }
        [NotMapped]
        [Display(Name = "Annee-Mois")]
        [Required(ErrorMessage = "Merci de saisir le mois")]
        public string ANNEEMOIS
        {
            //(ANNEE.ToString() + "-" + MOIS.ToString().PadLeft(2, '0'))
            //new DateTime(ANNEE, MOIS, 0).GetDateTimeFormats("yyyy-MM");
            get; set;/* => AnneeMois = value*/

        }
        [NotMapped]
        public Dictionary<string, List<string>> DetailsAbsence { get; set; }

        [NotMapped]
        public Dictionary<string, string> Details { get; set; }

    }
}
