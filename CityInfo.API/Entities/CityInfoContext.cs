using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Entities
{
    public class CityInfoContext : DbContext
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointsOfInterest { get; set; }

        public CityInfoContext(DbContextOptions<CityInfoContext> options)       // This is provided as an overload on the DBContext constructor. Better way than commented out below.
            : base(options)
        {
            //Database.EnsureCreated();     // Old method of creating a database. Much better to use Migrations (Nuget Package Manager Console: Add-Migration CityInfoDBInitialMigration)

            Database.Migrate();             // This will execute migrations and create the database if it isn't already there. Much better approach.
        }

        // One way
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("connectionstring");

        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}
