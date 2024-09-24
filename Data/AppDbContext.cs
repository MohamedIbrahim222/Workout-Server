   // Data/AppDbContext.cs
   using workOut.Models;
   using Microsoft.EntityFrameworkCore;

   namespace workOut.Data
   {
       public class AppDbContext : DbContext
       {
           public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
           {
           }

           public DbSet<User> Users { get; set; }
           public DbSet<Workout> Workouts { get; set; }
           public DbSet<Day> Days { get; set; }

           protected override void OnModelCreating(ModelBuilder modelBuilder)
           {
               modelBuilder.Entity<Workout>()
                   .HasMany(w => w.Days);
           }
       }
   }