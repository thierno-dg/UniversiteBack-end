using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.JeuxDeDonnees;
using UniversiteDomain.UseCases;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;
using UniversiteEFDataProvider.RepositoryFactories;
using UniversiteEFDataProvider.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services au conteneur
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(options =>
{
    options.ClearProviders();
    options.AddConsole();
});

builder.Services.AddScoped<GenererCSVNotesUseCase>();
builder.Services.AddScoped<UploadNotesUseCase>();
builder.Services.AddScoped<ValiderCSVNotesUseCase>();

// Connexion MySQL
string connectionString = builder.Configuration.GetConnectionString("MySqlConnection") 
    ?? throw new InvalidOperationException("Connection string 'MySqlConnection' not found.");

builder.Services.AddDbContext<UniversiteDbContext>(options =>
        options.UseMySQL(connectionString) 
);

// Sécurisation avec Identity
builder.Services.AddIdentityCore<UniversiteUser>()
    .AddRoles<UniversiteRole>()
    .AddEntityFrameworkStores<UniversiteDbContext>()
    .AddApiEndpoints();

builder.Services.AddScoped<RoleManager<UniversiteRole>>();
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();

// Enregistrement des repositories pour éviter les erreurs de dépendances
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IEtudiantRepository, EtudiantRepository>(); 
builder.Services.AddScoped<IUeRepository, UeRepository>(); 
builder.Services.AddScoped<IParcoursRepository, ParcoursRepository>(); 



// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Authentification & Autorisation
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

// Initialisation de la base de données
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<UniversiteDbContext>>();
    var context = scope.ServiceProvider.GetRequiredService<UniversiteDbContext>();

    logger.LogInformation("Initialisation de la base de données");

    if (app.Environment.IsDevelopment()) 
    {
        logger.LogInformation("Suppression de la BD si elle existe (Mode Développement)");
        await context.Database.EnsureDeletedAsync();
    }

    logger.LogInformation("Création de la BD et des tables");
    await context.Database.EnsureCreatedAsync();
}

// Chargement des données de test
ILogger logger1 = app.Services.GetRequiredService<ILogger<BasicBdBuilder>>();
logger1.LogInformation("Chargement des données de test");

using (var scope = app.Services.CreateScope())
{
    var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();   
    BdBuilder seedBd = new BasicBdBuilder(repositoryFactory); 
    await seedBd.BuildUniversiteBdAsync();
}

// Sécurisation
app.MapIdentityApi<UniversiteUser>();
app.MapControllers();

// Exécuter l'application
Debug.Assert(app != null, nameof(app) + " != null");
app.Run();
