using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RamMyERP3.Models
{
    public class RamDetailsPresence
    {
        public int ID { get; set; }
        [Column("RAMID")]
        public int RAMID { get; set; }
        public DateTime DATE_TRAVAILLE { get; set; }
        public int AffaireCollaborateurID { get; set; }
        [Column("nombreHeure")]
        public double NOMBREHEURE { get; set; }
        public AffaireCollaborateur affaireCollaborateur { get; set; }
        public Ram ram { get; set; }
    }
}
