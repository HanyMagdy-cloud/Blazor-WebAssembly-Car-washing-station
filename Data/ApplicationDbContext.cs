using Microsoft.EntityFrameworkCore;
using CarWashStation.Models;

namespace CarWashStation.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<BlockedSlot> BlockedSlots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Seed default services
            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, Name = "Utvändig tvätt", Description = "Professionell yttre rengöring", Price = 400m },
                new Service { Id = 2, Name = "In & utvändig tvätt", Description = "Komplett rengöring för både insida och utsida", Price = 750m },
                new Service { Id = 3, Name = "Rekond", Description = "Djupgående rengöring och polering", Price = 2500m },
                new Service { Id = 4, Name = "Tvätt + vax", Description = "Kombination av rengöring och skyddande vaxlager", Price = 1750m },
                new Service { Id = 5, Name = "Motortvätt", Description = "Rengöring av motorrummet", Price = 350m },
                new Service { Id = 6, Name = "Invändig tvätt", Description = "Grundlig dammsugning och avtorkning", Price = 400m },
                new Service { Id = 7, Name = "Däckförvaring + skifte", Description = "Bekväm säsongsförvaring inklusive skifte", Price = 950m },
                new Service { Id = 8, Name = "Däckskifte", Description = "Snabb och säker byte av hjul", Price = 350m },
                new Service { Id = 9, Name = "Lackförsegling", Description = "Långvarigt skydd mot repor och UV-ljus", Price = 2295m },
                new Service { Id = 10, Name = "Laga punktering", Description = "Professionell reparation av däckskador", Price = 350m },
                new Service { Id = 11, Name = "Däckmontering", Description = "Montering på fälg och balansering", Price = 300m }
            );
        }
    }
}
