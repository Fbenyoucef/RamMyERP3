using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RamMyERP3.Models
{
    public class r_absence_type
    {
		[Column("ID")]
		public int ID { get; set; }
		public string NOM { get; set; }
		public string CODE { get; set; }
		public DateTime DATE_CREATION { get; set; }
		public DateTime DATE_MODIFICATION { get; set; }
		public string USERMODIFICATION { get; set; }
	}
}
