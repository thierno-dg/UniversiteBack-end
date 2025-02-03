using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Create;

public class CreateEtudiantUseCase
{
    private readonly IRepositoryFactory _repositoryFactory;

    public CreateEtudiantUseCase(IRepositoryFactory repositoryFactory)
    {
        _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
    }

    public async Task<Etudiant> ExecuteAsync(string numEtud, string nom, string prenom, string email)
    {
        var etudiant = new Etudiant { NumEtud = numEtud, Nom = nom, Prenom = prenom, Email = email };
        return await ExecuteAsync(etudiant);
    }

    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules(etudiant);
        var et = await _repositoryFactory.EtudiantRepository().CreateAsync(etudiant);
        await _repositoryFactory.SaveChangesAsync();
        return et;
    }

    private async Task CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(etudiant.NumEtud, nameof(etudiant.NumEtud));
        ArgumentNullException.ThrowIfNull(etudiant.Email, nameof(etudiant.Email));

        // Vérifier si un étudiant avec le même numéro étudiant existe déjà
        var existe = await _repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.NumEtud.Equals(etudiant.NumEtud));
        if (existe.Any())
            throw new DuplicateNumEtudException($"{etudiant.NumEtud} - ce numéro d'étudiant est déjà affecté à un étudiant");

        // Vérification du format de l'email
        if (!CheckEmail.IsValidEmail(etudiant.Email))
            throw new InvalidEmailException($"{etudiant.Email} - Email mal formé");

        // Vérifier si l'email est déjà utilisé
        existe = await _repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.Email.Equals(etudiant.Email));
        if (existe.Any())
            throw new DuplicateEmailException($"{etudiant.Email} est déjà affecté à un étudiant");

        // Vérification de la longueur du nom
        if (etudiant.Nom.Length < 3)
            throw new InvalidNomEtudiantException($"{etudiant.Nom} incorrect - Le nom d'un étudiant doit contenir plus de 3 caractères");
    }

    public bool IsAuthorized(string role)
    {
        throw new NotImplementedException();
    }
}
