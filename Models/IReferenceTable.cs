using System;

namespace RamMyERP3.Models
{
    public interface IReferenceTable
    {
        public int ID { get; set; }
        public string NOM { get; set; }
        public string CODE { get; set; }
        public DateTime? DATE_CREATION { get; set; }
        public DateTime? DATE_MODIFICATION { get; set; }
        public string UTILISATEUR_MODIFICATION { get; set; }
        public int POSITION { get; set; }
    }
}
