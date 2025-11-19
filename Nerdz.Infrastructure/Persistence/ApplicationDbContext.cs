using Microsoft.EntityFrameworkCore;
using Nerdz.Domain.Entities;
using Nerdz.Infrastructure.Mapping;

namespace Nerdz.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> context)
            : base(context)
        {
        }

        public DbSet<User> Usuarios { get; set; }
        public DbSet<Group> Endereco { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMapping());
            modelBuilder.ApplyConfiguration(new GroupMapping());

            base.OnModelCreating(modelBuilder);
        }
    }
}
