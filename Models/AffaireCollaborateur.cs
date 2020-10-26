using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RamMyERP3.Models
{
    public class AffaireCollaborateur
    {
        public int ID { get; set; }
        public int COLLABORATEURID { get; set; }
        public int AFFAIREID { get; set; }
        public Affaire affaire { get; set; }
        public Collaborateur collaborateur { get; set; }
        public ICollection<RamDetailsPresence> ListeRamDetailsPresence { get; set; }

    }
}
