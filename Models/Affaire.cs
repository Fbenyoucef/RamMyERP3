using System;
using System.Collections.Generic;

namespace RamMyERP3.Models
{
    public class Affaire
    {
        public int ID { get; set; }
        public DateTime DATE_DEBUT { get; set; }
        public string NOM { get; set; }
        public int CLIENTID { get; set; }
        public Client CLIENT { get; set; }
        public int AFFAIRETYPEID { get; set; }
        public r_affaire_type AFFAIRETYPE { get; set; }
        public ICollection<AffaireCollaborateur> listeAffaireCollaborateur { get; set; }

    }
}
