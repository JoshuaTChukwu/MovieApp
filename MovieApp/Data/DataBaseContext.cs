using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieApp.SqlTables;

namespace MovieApp.Data
{
    public class DataBaseContext : IdentityDbContext<Users>
    {
        public DataBaseContext() { }


        public DataBaseContext(DbContextOptions<DataBaseContext> options)
            : base(options) { }
   
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }
        public virtual DbSet<MovieSearch> MovieSearch { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot config = builder.Build();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            int stringMaxLenght = 256;
            modelBuilder.Entity<Users>(x => x.Property(m => m.Id).HasMaxLength(stringMaxLenght));
        }
    }
}
