using System;
using System.Threading.Tasks;
using UniversiteDomain.Entities;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;

namespace UniversiteDomain.UseCases.EtudiantUseCases
{
    public class UpdateEtudiantUseCase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public UpdateEtudiantUseCase(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task ExecuteAsync(Etudiant etudiant)
        {
            if (etudiant == null) throw new ArgumentNullException(nameof(etudiant));
            var repository = _repositoryFactory.EtudiantRepository();
            await repository.UpdateAsync(etudiant);
        }

        public bool IsAuthorized(string role)
        {
            return role == "Admin" || role == "Professeur";
        }
    }
}