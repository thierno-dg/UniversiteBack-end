using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniversiteEFDataProvider.Repositories
{
    public class EtudiantRepository(UniversiteDbContext context) 
        : Repository<Etudiant>(context), IEtudiantRepository
    {
        public async Task<List<Etudiant>> GetAllAsync()
        {
            return await Context.Etudiants.Include(e => e.NotesObtenues)
                                           .ThenInclude(n => n.Ue)
                                           .ToListAsync();
        }

        public async Task<Etudiant?> GetByIdAsync(long id)
        {
            return await Context.Etudiants.Include(e => e.NotesObtenues)
                                           .ThenInclude(n => n.Ue)
                                           .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AffecterParcoursAsync(long idEtudiant, long idParcours)
        {
            ArgumentNullException.ThrowIfNull(Context.Etudiants);
            ArgumentNullException.ThrowIfNull(Context.Parcours);

            var etudiant = await Context.Etudiants.FindAsync(idEtudiant);
            var parcours = await Context.Parcours.FindAsync(idParcours);

            if (etudiant == null)
                throw new ArgumentException($"Aucun étudiant trouvé avec l'ID {idEtudiant}");

            if (parcours == null)
                throw new ArgumentException($"Aucun parcours trouvé avec l'ID {idParcours}");

            etudiant.ParcoursSuivi = parcours;
            await Context.SaveChangesAsync();
        }

        public async Task AffecterParcoursAsync(Etudiant etudiant, Parcours parcours)
        {
            await AffecterParcoursAsync(etudiant.Id, parcours.Id);
        }

        public async Task<Etudiant?> FindEtudiantCompletAsync(long idEtudiant)
        {
            ArgumentNullException.ThrowIfNull(Context.Etudiants);
            return await Context.Etudiants.Include(e => e.NotesObtenues)
                                           .ThenInclude(n => n.Ue)
                                           .FirstOrDefaultAsync(e => e.Id == idEtudiant);
        }
    }
}
