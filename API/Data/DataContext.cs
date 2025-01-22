using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public required DbSet<AppUser> Users { get; set; }
    public required DbSet<UserLike> Likes { get; set; }
    public required DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserLike>()
            .HasKey(key => new { key.SourceUserId, key.LikedUserId });

        modelBuilder.Entity<UserLike>()
            .HasOne(userLike => userLike.SourceUser)
            .WithMany(user => user.LikedUsers)
            .HasForeignKey(userLike => userLike.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserLike>()
            .HasOne(userLike => userLike.LikedUser)
            .WithMany(user => user.LikedByUsers)
            .HasForeignKey(userLike => userLike.LikedUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Message>()
            .HasOne(message => message.Recipient)
            .WithMany(user => user.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(message => message.Sender)
            .WithMany(user => user.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
