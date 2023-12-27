using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Data.DataContext
{
    public class AirlineDbContext  : IdentityDbContext
    { 

        public AirlineDbContext(DbContextOptions<AirlineDbContext> options) : base(options)
        {

        }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<Flight> Flights { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ticket>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Flight>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
        }
    }
}
