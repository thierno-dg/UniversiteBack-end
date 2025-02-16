using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniversiteEFDataProvider.Repositories
{
    public class EtudiantRepository(UniversiteDbContext context) 
        : Repository<Etudiant>(context), IEtudiantRepository
    {
        public async Task<List<Etudiant>> GetAllAsync()
        {
            return await Context.Etudiants
                .Include(e => e.NotesObtenues)
                .ThenInclude(n => n.Ue)
                .AsNoTracking() 
                .ToListAsync();
        }

        public async Task<List<Etudiant>> GetAllAsync(string numeroUe)
        {
            return await Context.Etudiants
                .Where(e => e.NotesObtenues.Any(n => n.Ue.NumeroUe == numeroUe)) 
                .Include(e => e.NotesObtenues) 
                .ThenInclude(n => n.Ue)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Etudiant?> GetByIdAsync(long id)
        {
            return await Context.Etudiants
                .Include(e => e.NotesObtenues)
                .ThenInclude(n => n.Ue)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Etudiant?> FindEtudiantCompletAsync(long idEtudiant)
        {
            return await Context.Etudiants
                .Include(e => e.NotesObtenues)
                .ThenInclude(n => n.Ue)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == idEtudiant);
        }
        public async Task<List<Etudiant>> GetEtudiantsParUeAsync(string numeroUe)
        {
            return await Context.Etudiants
                .Where(e => e.NotesObtenues.Any(n => n.Ue.NumeroUe == numeroUe)) 
                .Include(e => e.NotesObtenues) 
                .ThenInclude(n => n.Ue)
                .AsNoTracking()
                .ToListAsync();
        }
        
        public async Task<Etudiant?> FindByNumEtudAsync(string numEtud)
        {
            return await Context.Etudiants
                .Include(e => e.NotesObtenues)
                .ThenInclude(n => n.Ue)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.NumEtud == numEtud);
        }


    }
}