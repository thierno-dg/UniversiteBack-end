using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.JeuxDeDonnees;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;
using UniversiteEFDataProvider.RepositoryFactories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(options =>
{
    options.ClearProviders();
    options.AddConsole();
});

// Configuration de la connexion à MySql
string connectionString = builder.Configuration.GetConnectionString("MySqlConnection") 
    ?? throw new InvalidOperationException("Connection string 'MySqlConnection' not found.");
    
builder.Services.AddDbContext<UniversiteDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 31)))
);

// Sécurisation avec Identity et gestion des rôles
builder.Services.AddIdentityCore<UniversiteUser>()
    .AddRoles<UniversiteRole>() // ✅ Correction ici : utilisation de UniversiteRole au lieu de IdentityRole
    .AddEntityFrameworkStores<UniversiteDbContext>()
    .AddApiEndpoints();

// ✅ Ajout de l'enregistrement de RoleManager<UniversiteRole>
builder.Services.AddScoped<RoleManager<UniversiteRole>>();

// Enregistrement de la RepositoryFactory
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();

// Configuration de l'authentification et l'autorisation
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);

// Création de l'application
var app = builder.Build();

// Configuration du serveur Web
app.UseHttpsRedirection();
app.MapControllers();

// Configuration de Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Initialisation de la base de données
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<UniversiteDbContext>>();
    var context = scope.ServiceProvider.GetRequiredService<UniversiteDbContext>();
    
    logger.LogInformation("Initialisation de la base de données");
    logger.LogInformation("Suppression de la BD si elle existe");
    await context.Database.EnsureDeletedAsync();
    
    logger.LogInformation("Création de la BD et des tables à partir des entités");
    await context.Database.EnsureCreatedAsync();
}

// Chargement des données de test
ILogger logger1 = app.Services.GetRequiredService<ILogger<BdBuilder>>();
logger1.LogInformation("Chargement des données de test");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UniversiteDbContext>();
    var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();   
    
    BdBuilder seedBD = new BasicBdBuilder(repositoryFactory);
    await seedBD.BuildUniversiteBdAsync();
}

// Sécurisation
app.UseAuthorization();
app.MapIdentityApi<UniversiteUser>();

// Exécution de l'application
Debug.Assert(app != null, nameof(app) + " != null");
app.Run();
