using Server_advanced_Baturina.Models;
using Server_Ad_Baturina.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace Server_Ad_Baturina.IntegrationTest;

public static class TestDataSeeder
{
    public static void SeedTestData(Context context, IPasswordHasher<UserEntity> passwordHasher = null)
    {
        passwordHasher ??= new PasswordHasher<UserEntity>();

        var user1 = new UserEntity
        {
            Id = Guid.NewGuid(),
            Name = "Alice",
            Email = "alice@example.com",
            Password = passwordHasher.HashPassword(null, "Passsword12345&9"),
            Role = "Admin",
            Avatar = null
        };

        var user2 = new UserEntity
        {
            Id = Guid.NewGuid(),
            Name = "Bob",
            Email = "bob@example.com",
            Password = passwordHasher.HashPassword(null, "hashedpassword2T#"),
            Role = "User",
            Avatar = null
        };

        context.Users.AddRange(user1, user2);

        var tagNews = new TagsEntity
        {
            Id = Guid.NewGuid(),
            Title = "News"
        };

        var tagTech = new TagsEntity
        {
            Id = Guid.NewGuid(),
            Title = "Technology"
        };

        var tagSports = new TagsEntity
        {
            Id = Guid.NewGuid(),
            Title = "Sports"
        };

        context.Tags.AddRange(tagNews, tagTech, tagSports);

        var news1 = new NewsEntity
        {
            Id = Guid.NewGuid(),
            Title = "Breaking News",
            Description = "Something big happened today.",
            Image = null,
            UserId = user1.Id,
            User = user1,
            Tags = new List<TagsEntity> { tagNews, tagTech }
        };

        var news2 = new NewsEntity
        {
            Id = Guid.NewGuid(),
            Title = "Sports Update",
            Description = "Latest sports results.",
            Image = null,
            UserId = user2.Id,
            User = user2,
            Tags = new List<TagsEntity> { tagSports }
        };

        context.News.AddRange(news1, news2);

        context.SaveChanges();
    }
}