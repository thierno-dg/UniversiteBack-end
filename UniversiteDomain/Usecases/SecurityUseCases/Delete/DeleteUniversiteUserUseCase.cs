
using System;
using System.Threading.Tasks;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;

namespace UniversiteDomain.UseCases.EtudiantUseCases
{
    public class DeleteUniversiteUserUseCase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public DeleteUniversiteUserUseCase(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task ExecuteAsync(long userId)
        {
            var repository = _repositoryFactory.UniversiteUserRepository();
            await repository.DeleteAsync(userId);
        }

        public bool IsAuthorized(string role)
        {
            return role == "Admin";
        }
    }
}
