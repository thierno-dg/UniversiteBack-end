
using System;
using System.Threading.Tasks;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases
{
    public class UpdateUniversiteUserUseCase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public UpdateUniversiteUserUseCase(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task ExecuteAsync(IUniversiteUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var repository = _repositoryFactory.UniversiteUserRepository();
            await repository.UpdateAsync(user);
        }

        public bool IsAuthorized(string role)
        {
            return role == "Admin";
        }
    }
}
