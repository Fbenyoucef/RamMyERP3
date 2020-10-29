using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RamMyERP3.Models
{
    public class Collaborateur
    {
        public int ID { get; set; }
        [DisplayName("Nom")]
        public string NOM { get; set; }
        [DisplayName("Prénom")]
        public string PRENOM { get; set; }

        public string USERID { get; set; }

        [DisplayName("Date de naissance")]
        public DateTime DATENAISSANCE { get; set; }
        public ICollection<Ram> LISTERAM { get; set; }
        public ICollection<AffaireCollaborateur> ListeAffaireCollaborateur { get; set; }


    }
}
