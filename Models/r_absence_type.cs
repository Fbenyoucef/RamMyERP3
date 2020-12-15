using MyErp.MyTagHelpers;
using RamMyERP3.Helpers.Entite;
using System;
using System.ComponentModel.DataAnnotations;

namespace RamMyERP3.Models
{
    /// <summary>
    /// table de référence r_absence_type : RH c'est la fonction 
    /// </summary>
    [Fonction(fonction: "RH", nomTable: "Type d'absence")]
    public class r_absence_type : IReferenceTable
    {
        [Display(Name = "Id")]
        [Key()]
        [Lister(Cacher = true, IsReadOnly = true)]
        public int ID { get; set; }
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
        [Display(Name = "Position")]
        [Lister(IsReadOnly = true, Cacher = true)]
        public int POSITION { get; set; }
    }
}
