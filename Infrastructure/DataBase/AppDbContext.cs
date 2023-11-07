using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Enums;
using System.Text;
using System.Security.Cryptography;

namespace Infrastructure.DataBase;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions opts) : base(opts) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<Paste> Postes => Set<Paste>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureIndexes(modelBuilder);
        SeedData(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .HasConversion(
                u => u.Value,
                u => new Username(u));

        modelBuilder.Entity<User>()
            .Property(u => u.Password)
            .HasConversion(
                p => p.Value,
                p => new Password(p));

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasConversion(
                e => e.Value,
                e => new Email(e, true));
    }

    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<Paste>()
             .HasOne(p => p.User)
             .WithMany(u => u.Pastes)
             .HasForeignKey(p => p.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<User>()
            .Navigation(u => u.Pastes).AutoInclude();
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var user = User.Create(new Username("SuperAdmin2077CP"),
            new Password(HashPassword("qwerty28042002")),
            new Email("string@mail.com", true),
            Role.Admin,
            "ConfirmToken");

        var poste = new Paste
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

        modelBuilder.Entity<User>().HasData(user);
        modelBuilder.Entity<Paste>().HasData(poste);
    }


    private string HashPassword(string password)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        return hash;
    }
}
