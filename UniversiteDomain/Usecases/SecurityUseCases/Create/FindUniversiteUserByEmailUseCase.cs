using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

public class FindUniversiteUserByEmailUseCase
{
    private readonly IRepositoryFactory _repositoryFactory;

    public FindUniversiteUserByEmailUseCase(IRepositoryFactory repositoryFactory)
    {
        _repositoryFactory = repositoryFactory;
    }

    public async Task<IUniversiteUser?> ExecuteAsync(string email)
    {
        var user = await _repositoryFactory.UniversiteUserRepository().FindByEmailAsync(email);
        return user as IUniversiteUser;
    }
}