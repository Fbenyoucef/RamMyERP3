using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RamMyERP3.Models
{
    public class ReferenceDomaine
    {
        public ReferenceDomaine()
        {
            ReferenceTable = new Dictionary<string, List<string>>();
        }
        public Dictionary<string, List<string>> ReferenceTable { get; set; }
    }
}
