using System;
using System.Reflection;

namespace RamMyERP3.Models
{
    public class ProprieteInfos
    {
        public string Nom { get; set; }
        public Type Type { get; set; }
        public bool Visibilite { get; set; }
        public PropertyInfo Originale { get; set; }

    }
}
