using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ManageNotes.Data
{
    public class ApplicationContext:DbContext
    {
        private IConfiguration Configuration;
        public ApplicationContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }
        public DbSet<Note> Notes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Payment> Payments { get; set; }
        
        public DbSet<Log> Logs { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>().HasIndex(x => x.Syst_Code);
            base.OnModelCreating(modelBuilder);
        }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer((Configuration.GetConnectionString("defualt")));
            base.OnConfiguring(optionsBuilder);
        }*/
    }
}