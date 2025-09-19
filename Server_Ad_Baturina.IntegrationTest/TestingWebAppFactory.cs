using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Server_Ad_Baturina.Models.Entities;
using Server_advanced_Baturina.Models;
using Microsoft.AspNetCore.Identity;

namespace Server_Ad_Baturina.IntegrationTest;

public class TestingWebAppFactory<T> : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("JWT", "supersecretkey1234567890supersecret!");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "yourissuer");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "youraudience");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<Context>));

            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var dbName = Guid.NewGuid().ToString();

            services.AddDbContext<Context>(options =>
            {
                options.UseInMemoryDatabase(dbName);
                options.UseInternalServiceProvider(serviceProvider);
            });

            // Добавляем PasswordHasher
            services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<Context>();
                var passwordHasher = scopedServices.GetRequiredService<IPasswordHasher<UserEntity>>();

                try
                {
                    db.Database.EnsureCreated();
                    TestDataSeeder.SeedTestData(db, passwordHasher);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error when creating the test database", ex);
                }
            }
        });
    }
}