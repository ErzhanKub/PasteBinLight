using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.DataBase
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions opts) : base(opts) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<Poste> Postes => Set<Poste>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureIndexes(modelBuilder);
            SeedData(modelBuilder);
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<Poste>()
                 .HasOne(p => p.User)
                 .WithMany(u => u.Postes)
                 .HasForeignKey(p => p.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .Navigation(u => u.Postes).AutoInclude();
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var userId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "string",
                Password = "473287f8298dba7163a897908958f7c0eae733e25d2e027992ea2edc9bed2fa8",
                Email = "string@mail.com",
                Role = Role.User,
            };
            var admin = new User
            {
                Id = adminId,
                Username = "qwerty",
                Password = "473287f8298dba7163a897908958f7c0eae733e25d2e027992ea2edc9bed2fa8",
                Email = "erzhan@mail.com",
                Role = Role.Admin,
            };

            var posteId = Guid.NewGuid();
            var poste = new Poste
            {
                Id = posteId,
                Title = "My day",
                Url = new Uri("https://www.youtube.com/"),
                DateCreated = DateTime.Now,
                DeadLine = DateTime.Now.AddMonths(1),
                Likes = 183,
                DisLikes = 13,
                IsPrivate = false,
                UserId = userId,
            };

            modelBuilder.Entity<User>().HasData(user);
            modelBuilder.Entity<User>().HasData(admin);
            modelBuilder.Entity<Poste>().HasData(poste);
        }
    }
}