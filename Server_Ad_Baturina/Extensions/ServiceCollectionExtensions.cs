using DotNetEnv;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server_Ad_Baturina.Interfaces;
using Server_Ad_Baturina.Models.Entities;
using Server_Ad_Baturina.Repository;
using Server_Ad_Baturina.Services;
using Server_Ad_Baturina.Validations;
using Server_advanced_Baturina.Automapper;
using Server_advanced_Baturina.Interfaces;
using Server_advanced_Baturina.Models;
using Server_advanced_Baturina.Repository;
using Server_advanced_Baturina.Services;
using System.Text;

namespace Server_Ad_Baturina.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        Env.Load(".env");
        var connection = Environment.GetEnvironmentVariable("DATABASE");
        services.AddDbContext<Context>((options => options.UseNpgsql(connection)));

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<INewsRepository, NewsRepository>();

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IUserService, UserService>();

        services.AddScoped<INewsService, NewsService>();

        services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();

        services.AddScoped<IFileService, FileService>();

        services.AddHttpContextAccessor();

        services.AddAutoMapper(typeof(ApiMappingProfile));

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen();

        services.AddControllers();

        services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

        services.AddValidatorsFromAssemblyContaining<PutUserRequestValidator>();

        services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();

        services.AddValidatorsFromAssemblyContaining<SignInRequestValidator>();

        services.AddValidatorsFromAssemblyContaining<NewsRequestValidator>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Env.GetString("JWT_ISSUER"),
                    ValidAudience = Env.GetString("JWT_AUDIENCE"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                        .GetBytes(Env.GetString("JWT")))
                };
            });

        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "2.0" });
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                    }
                });
            });

        return services;
    }
}