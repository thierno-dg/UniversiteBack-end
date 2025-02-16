using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversiteEFDataProvider.Repositories
{
    public class NoteRepository(UniversiteDbContext context) : Repository<Note>(context), INoteRepository
    {
        // ✅ Ajoute une nouvelle note
        public async Task AddAsync(Note note)
        {
            await Context.Notes.AddAsync(note);
            await Context.SaveChangesAsync();
        }

        // ✅ Récupère une note spécifique pour un étudiant et une UE
        public async Task<Note?> GetByEtudiantAndUEAsync(string etudiantNum, string numeroUe)
        {
            return await Context.Notes
                                .Include(n => n.Ue)
                                .FirstOrDefaultAsync(n => n.Etudiant.NumEtud == etudiantNum && n.Ue.NumeroUe == numeroUe);
        }

        // ✅ Récupère toutes les notes d’un étudiant
        public async Task<List<Note>> GetByEtudiantAsync(string etudiantNum)
        {
            return await Context.Notes
                                .Where(n => n.Etudiant.NumEtud == etudiantNum)
                                .Include(n => n.Ue)
                                .AsNoTracking()
                                .ToListAsync();
        }

        // ✅ Ajoute ou met à jour une note pour un étudiant dans une UE
        public async Task AjouterOuMettreAJourNoteAsync(long idEtudiant, long idUe, long valeurNote)
        {
            ArgumentNullException.ThrowIfNull(Context.Notes);
            ArgumentNullException.ThrowIfNull(Context.Etudiants);
            ArgumentNullException.ThrowIfNull(Context.Ues);

            // Vérifie si une note existe déjà pour cet étudiant et cette UE
            var note = await Context.Notes.FirstOrDefaultAsync(n => n.EtudiantId == idEtudiant && n.UeId == idUe);
            if (note != null)
            {
                // Met à jour la note existante
                note.Valeur = valeurNote;
                Context.Notes.Update(note);
            }
            else
            {
                // Crée une nouvelle note
                note = new Note
                {
                    EtudiantId = idEtudiant,
                    UeId = idUe,
                    Valeur = valeurNote
                };
                await Context.Notes.AddAsync(note);
            }
            
            await Context.SaveChangesAsync();
        }

        // ✅ Récupère toutes les notes d'un étudiant
        public async Task<List<Note>> ObtenirNotesParEtudiantAsync(long idEtudiant)
        {
            ArgumentNullException.ThrowIfNull(Context.Notes);

            return await Context.Notes
                .Where(n => n.EtudiantId == idEtudiant)
                .Include(n => n.Ue)
                .ToListAsync();
        }
        
        // ✅ Implémente GetNotesByUeAsync pour récupérer toutes les notes d'une UE
        public async Task<List<Note>> GetNotesByUeAsync(string numeroUe)
        {
            return await Context.Notes
                .Where(n => n.Ue.NumeroUe == numeroUe) // Filtrer par l'UE
                .Include(n => n.Etudiant) // Charger les étudiants liés aux notes
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
