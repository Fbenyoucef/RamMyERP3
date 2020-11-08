using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RamMyERP3.Models
{
    public class RamDetailsAbsence
    {
        public int ID { get; set; }
        [Display(Name = "Date")]
        public DateTime DATE_ABSENCE { get; set; }
        public int RAMID { get; set; }
        public int R_absence_typeID { get; set; }
        [Column("nombreHeures")]
        public double NOMBREHEURES { get; set; }
        public r_absence_type r_absence_type { get; set; }


    }
}
