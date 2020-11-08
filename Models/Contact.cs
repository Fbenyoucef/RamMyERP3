namespace RamMyERP3.Models
{
    public class Contact
    {
        public int ID { get; set; }
        public string NOM { get; set; }
        public string PRENOM { get; set; }
        public int TELEPHONE { get; set; }
        public string EMAIL { get; set; }
        public int client_ID { get; set; }
    }
}
