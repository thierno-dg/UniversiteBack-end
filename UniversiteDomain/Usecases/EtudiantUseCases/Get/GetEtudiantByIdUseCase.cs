using System.Threading.Tasks;
using UniversiteDomain.Entities;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;

namespace UniversiteDomain.UseCases.EtudiantUseCases
{
    public class GetEtudiantByIdUseCase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public GetEtudiantByIdUseCase(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<Etudiant?> ExecuteAsync(long id)
        {
            var repository = _repositoryFactory.EtudiantRepository();
            return await repository.GetByIdAsync(id);
        }
        
        public bool IsAuthorized(string role, IUniversiteUser user, long id)
        {
            // Autorisation pour les admins et professeurs
            if (role == "Admin" || role == "Professeur")
                return true;

            // Un étudiant ne peut accéder qu'à ses propres informations
            if (role == "Etudiant" && user?.Etudiant?.Id == id)
                return true;

            return false; // Sinon, accès refusé
        }


    }
}