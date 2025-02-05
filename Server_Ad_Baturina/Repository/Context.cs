namespace Server_advanced_Baturina.Models;
using Microsoft.EntityFrameworkCore;
using Server_Ad_Baturina.Models.Entities;

public class Context : DbContext
{
    public Context(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<NewsEntity> News{ get; set; }
    public DbSet<TagsEntity> Tags{ get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Name)
                .HasMaxLength(25);

            entity.Property(e => e.Role)
                .HasMaxLength(25);
        });

        modelBuilder.Entity<NewsEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .HasMaxLength(130);

            entity.Property(e => e.Description)
                .HasMaxLength(160);
        });

        modelBuilder.Entity<TagsEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .HasMaxLength(160);
        });

        modelBuilder.Entity<NewsEntity>()
                .HasOne(n => n.User)
                .WithMany() 
                .HasForeignKey(n => n.UserId)  
                .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NewsEntity>()
            .HasMany(n => n.Tags)
            .WithMany(t => t.News)
            .UsingEntity<Dictionary<string, object>>(
                "NewsTags",
                r => r.HasOne<TagsEntity>().WithMany().HasForeignKey("TagId"),
                l => l.HasOne<NewsEntity>().WithMany().HasForeignKey("NewsId"),
                je =>
                {
                    je.HasKey("TagId", "NewsId");
                    je.ToTable("NewsTags");
                }
            );
    }
}