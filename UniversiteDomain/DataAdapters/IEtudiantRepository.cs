using System.Linq.Expressions;
using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface IEtudiantRepository : IRepository<Etudiant>
{
    public Task<Etudiant?> FindEtudiantCompletAsync(long idEtudiant);
    Task<List<Etudiant>> GetAllAsync();
    Task<Etudiant?> GetByIdAsync(long id);
}