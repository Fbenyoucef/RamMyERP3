using MyErp.MyTagHelpers;
using RamMyERP3.Helpers.Entite;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RamMyERP3.Models
{
    [Fonction("FI", "Villes", "r_pays")]
    public class r_ville : IReferenceTable
    {
        [Display(Name = "Id")]
        [Key()]
        [Lister(Cacher = true, IsReadOnly = true)]
        public int ID { get; set; }
        [Display(Name = "Pays")]
        public int R_PAYSID { get; set; }
        [Display(Name = "Nom")]
        public string NOM { get; set; }
        [Display(Name = "Code")]
        public string CODE { get; set; }
        [Lister(IsReadOnly = true)]
        [Display(Name = "Date Création")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DATE_CREATION { get; set; }
        [Lister(IsReadOnly = true)]
        [Display(Name = "Date Modification")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DATE_MODIFICATION { get; set; }
        [Display(Name = "Utilisateur Modification")]
        [Lister(IsReadOnly = true)]
        public string UTILISATEUR_MODIFICATION { get; set; }
        [Display(Name = "Code Postal")]
        public int CODE_POSTAL { get; set; }
        [Lister(IsList = true, DisplayChamp = "NOM", Cacher = true)]
        [Display(Name = "Pays")]
        [NotMapped]
        public r_pays R_PAYS { get; set; }
        [Display(Name = "Position")]
        [Lister(IsReadOnly = true, Cacher = true)]
        public int POSITION { get; set; }

    }
}
