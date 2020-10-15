using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RamMyERP3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RamMyERP3.DataContext
{
    public class IdentityUserDbContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityUserDbContext(DbContextOptions<IdentityUserDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }


}
