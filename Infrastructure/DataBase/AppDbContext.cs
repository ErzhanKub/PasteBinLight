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
                 .HasForeignKey(p => p.UserId);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var userId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "Mark",
                Password = "qwerty",
                Email = "mark@mail.com",
                Role = Role.User,
            };
            var admin = new User
            {
                Id = adminId,
                Username = "erzhan",
                Password = "string",
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
                User = user,
                UserId = userId,
            };

            modelBuilder.Entity<User>().HasData(user);
            modelBuilder.Entity<User>().HasData(admin);
            modelBuilder.Entity<Poste>().HasData(poste);
        }
    }
}