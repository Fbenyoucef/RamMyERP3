using MyErp.MyTagHelpers;
using RamMyERP3.Helpers.Entite;
using System;
using System.ComponentModel.DataAnnotations;

namespace RamMyERP3.Models
{
    [Fonction("FI", "Villes", "r_pays")]
    public class r_ville : IReferenceTable
    {
        [Display(Name = "Id")]
        [Key()]
        public int ID { get; set; }
        [Display(Name = "Pays")]
        [Lister(Cacher =true)]
        public int R_PAYSID { get; set; }
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

        [Display(Name = "Code Postal")]
        public int CODE_POSTAL { get; set; }
        [Lister(IsList = true,DisplayChamp = "NOM")]
        [Display(Name = "Pays")]
        public r_pays R_PAYS { get; set; }
    }
}
