using Microsoft.AspNetCore.Mvc;
using RamMyERP3.Helpers.Entite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RamMyERP3.Models
{
   [Domaine("RH")]
    public class r_absence_type:IReferenceTable
    {
		public int ID { get; set; }
		public string NOM { get; set; }
		public string CODE { get; set; }
		public DateTime DATE_CREATION { get; set; }
		public DateTime DATE_MODIFICATION { get; set; }
		public string USER_MODIFICATION { get; set; }
	}
}
