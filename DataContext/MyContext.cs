using Microsoft.EntityFrameworkCore;
using RamMyERP3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RamMyERP3.DataContext
{
    public class MyContext : DbContext
    {
        public MyContext()
        {

        }
        public MyContext(DbContextOptions opt) : base(opt) { }
        public DbSet<r_absence_type> r_absence_type { get; set; }
        public DbSet<Affaire> Affaire { get; set; }
        public DbSet<AffaireCollaborateur> AffaireCollaborateur { get; set; }
        public DbSet<r_affaire_type> r_affaire_type { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Collaborateur> Collaborateur { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<r_fonction> r_fonction { get; set; }
        public DbSet<Ram> Ram { get; set; }
        public DbSet<RamDetailsAbsence> RamDetailsAbsence { get; set; }
        public DbSet<RamDetailsPresence> RamDetailsPresence { get; set; }
        public DbSet<Responsable_client> Responsable_client { get; set; }

    }
}
