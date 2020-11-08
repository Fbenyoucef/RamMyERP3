using RamMyERP3.Helpers.Entite;
using System;
using System.ComponentModel.DataAnnotations;

namespace RamMyERP3.Models
{
    [Fonction("RH", "Compétence")]
    public class r_competence
    {
        [Display(Name = "Id")]
        [Key()]
        public int ID { get; set; }
        [Display(Name = "Nom")]
        public string NOM { get; set; }
        [Display(Name = "Code")]
        public string CODE { get; set; }
        [Display(Name = "Date Création")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DATE_CREATION { get; set; }
        [Display(Name = "Date Modification")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DATE_MODIFICATION { get; set; }
        [Display(Name = "Utilisateur Modification")]
        public string UTILISATEUR_MODIFICATION { get; set; }
    }
}
