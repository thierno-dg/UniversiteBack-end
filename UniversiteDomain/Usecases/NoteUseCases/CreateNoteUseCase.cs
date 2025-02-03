using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.UeExceptions;
using System.Linq;
using System.Threading.Tasks;

public class CreateNoteUseCase
{
    private readonly IRepositoryFactory _repositoryFactory;

    public CreateNoteUseCase(IRepositoryFactory repositoryFactory)
    {
        _repositoryFactory = repositoryFactory;
    }

    public async Task<Note> ExecuteAsync(long etudiantId, long ueId, float valeur)
    {
        // Vérifications des règles métier
        await CheckBusinessRules(etudiantId, ueId, valeur);

        // Création de la note
        var note = new Note
        {
            EtudiantId = etudiantId,
            UeId = ueId,
            Valeur = valeur
        };

        // Sauvegarde de la note dans le repository
        var createdNote = await _repositoryFactory.NoteRepository().CreateAsync(note);
        await _repositoryFactory.NoteRepository().SaveChangesAsync();

        return createdNote;
    }

    private async Task CheckBusinessRules(long etudiantId, long ueId, float valeur)
    {
        // Vérification des paramètres
        if (etudiantId <= 0)
        {
            throw new ArgumentException("L'ID de l'étudiant est invalide.", nameof(etudiantId));
        }

        if (ueId <= 0)
        {
            throw new ArgumentException("L'ID de l'UE est invalide.", nameof(ueId));
        }

        if (valeur < 0 || valeur > 20)
        {
            throw new InvalidNoteException($"La valeur de la note doit être comprise entre 0 et 20. Valeur actuelle : {valeur}");
        }

        // Vérification que l'étudiant existe
        var etudiant = await _repositoryFactory.EtudiantRepository().FindAsync(etudiantId);
        if (etudiant == null)
        {
            throw new EtudiantNotFoundException($"Étudiant avec l'ID {etudiantId} introuvable.");
        }

        // Vérification que l'UE existe
        var ue = await _repositoryFactory.UeRepository().FindAsync(ueId);
        if (ue == null)
        {
            throw new UeNotFoundException($"UE avec l'ID {ueId} introuvable.");
        }

        // Vérification qu'une note n'existe pas déjà pour cet étudiant dans cette UE
        var existingNotes = await _repositoryFactory.NoteRepository().FindByConditionAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);
        if (existingNotes.Any())
        {
            throw new DuplicateNotePourUeException($"Une note existe déjà pour l'étudiant (ID: {etudiantId}) dans l'UE (ID: {ueId}).");
        }

        // Vérification que l'UE est enseignée dans le parcours de l'étudiant
        if (etudiant.ParcoursSuivi == null || etudiant.ParcoursSuivi.UesEnseignees.All(u => u.Id != ueId))
        {
            throw new InvalidOperationException($"L'UE (ID: {ueId}) n'est pas enseignée dans le parcours de l'étudiant (ID: {etudiantId}).");
        }
    }
}
