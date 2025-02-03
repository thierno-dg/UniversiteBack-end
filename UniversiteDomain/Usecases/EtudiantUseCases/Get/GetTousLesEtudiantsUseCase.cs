using System.Collections.Generic;
using System.Threading.Tasks;
using UniversiteDomain.Entities;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;

namespace UniversiteDomain.UseCases.EtudiantUseCases
{
    public class GetTousLesEtudiantsUseCase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public GetTousLesEtudiantsUseCase(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<List<Etudiant>> ExecuteAsync()
        {
            var repository = _repositoryFactory.EtudiantRepository();
            return await repository.GetAllAsync();
        }
        
        public bool IsAuthorized(string role)
        {
            return role == "Admin" || role == "Professeur";
        }

    }
    
}