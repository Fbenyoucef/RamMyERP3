using RamMyERP3.Helpers.Entite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RamMyERP3.Models
{
    [Domaine("RH")]
    public class r_pays:IReferenceTable
    {
        public int ID { get; set; }
        public string NOM { get; set; }
        public string CODE { get; set; }
        public DateTime DATE_CREATION { get; set; }
        public DateTime DATE_MODIFICATION { get; set; }
        public string USER_MODIFICATION { get; set; }
    }
}
