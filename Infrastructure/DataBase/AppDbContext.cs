using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Enums;
using System.Text;
using System.Security.Cryptography;

namespace Infrastructure.DataBase
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions opts) : base(opts) { }

        // Define DbSets for User and Record entities
        public DbSet<User> Users => Set<User>();
        public DbSet<Record> Records => Set<Record>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure indexes and relationships
            ConfigureEntityRelationships(modelBuilder);

            // Seed initial data
            SeedInitialData(modelBuilder);

            // Configure property conversions for User entity
            ConfigureUserPropertyConversions(modelBuilder);
        }

        // Configure indexes and relationships
        private void ConfigureEntityRelationships(ModelBuilder modelBuilder)
        {
            // Ensure Username is unique
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            // Configure one-to-many relationship between User and Record entities
            modelBuilder.Entity<Record>()
                .HasOne(p => p.User)
                .WithMany(u => u.Records)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Auto-include navigation property Records when querying User
            modelBuilder.Entity<User>()
                .Navigation(u => u.Records).AutoInclude();
        }

        // Seed initial data
        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            var user = User.CreateUser(new Username("SuperAdmin2077CP"),
                new Password(HashPassword("qwerty28042002")),
                new Email("string@mail.com", true),
                Role.Admin,
                "ConfirmToken");

            var record = new Record
            {
                Id = Guid.NewGuid(),
                Title = "My day",
                Url = new Uri("https://www.youtube.com/"),
                DateCreated = DateTime.Now,
                DeadLine = DateTime.Now.AddMonths(1),
                Likes = 183,
                DisLikes = 13,
                IsPrivate = false,
                UserId = user.Id,
            };

            // Add initial data to User and Record entities
            modelBuilder.Entity<User>().HasData(user);
            modelBuilder.Entity<Record>().HasData(record);
        }

        // Hash a password
        private string HashPassword(string password)
        {
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            return hash;
        }

        // Configure property conversions for User entity
        private void ConfigureUserPropertyConversions(ModelBuilder modelBuilder)
        {
            // Configure conversion for Username property
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .HasConversion(
                    u => u.Value,
                    u => new Username(u));

            // Configure conversion for Password property
            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .HasConversion(
                    p => p.Value,
                    p => new Password(p));

            // Configure conversion for Email property
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasConversion(
                    e => e.Value,
                    e => new Email(e, true));
        }
    }
}
