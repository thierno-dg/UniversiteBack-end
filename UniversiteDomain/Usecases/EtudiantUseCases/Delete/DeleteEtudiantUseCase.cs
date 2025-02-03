using System;
using System.Threading.Tasks;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete
{
    public class DeleteEtudiantUseCase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public DeleteEtudiantUseCase(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task ExecuteAsync(long etudiantId)
        {
            if (etudiantId <= 0) 
                throw new ArgumentException("L'ID de l'étudiant doit être valide.", nameof(etudiantId));

            var repository = _repositoryFactory.EtudiantRepository();
            var etudiant = await repository.GetByIdAsync(etudiantId);

            if (etudiant == null)
                throw new InvalidOperationException($"Aucun étudiant trouvé avec l'ID {etudiantId}.");

            await repository.DeleteAsync(etudiantId);
        }

        public bool IsAuthorized(string role)
        {
            return role == "Admin" || role == "Professeur";
        }
    }
}