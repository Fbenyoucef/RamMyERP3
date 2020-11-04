using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RamMyERP3.Helpers.Entite
{
    public class DomaineAttribute:Attribute
    {
        public DomaineAttribute (string domaine)
        {
            Domaine = domaine;
        }
        public string Domaine { get; set; }
      
    }
}
