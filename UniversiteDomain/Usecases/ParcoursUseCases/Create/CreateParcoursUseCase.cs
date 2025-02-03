using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Exceptions.ParcoursExceptions;

public class CreateParcoursUseCase
{
    private readonly IRepositoryFactory _repositoryFactory;

    public CreateParcoursUseCase(IRepositoryFactory repositoryFactory)
    {
        _repositoryFactory = repositoryFactory;
    }

    public async Task<Parcours> ExecuteAsync(string nomParcours, int anneeFormation)
    {
        var parcours = new Parcours { NomParcours = nomParcours, AnneeFormation = anneeFormation };
        return await ExecuteAsync(parcours);
    }

    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours pr = await _repositoryFactory.ParcoursRepository().CreateAsync(parcours);
        await _repositoryFactory.ParcoursRepository().SaveChangesAsync();
        return pr;
    }

    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);

        var existe = await _repositoryFactory.ParcoursRepository()
            .FindByConditionAsync(p => p.NomParcours.Equals(parcours.NomParcours));

        if (existe != null && existe.Count > 0)
        {
            throw new DuplicateParcoursException(parcours.NomParcours + " Ce parcours existe déjà");
        }

        if (parcours.AnneeFormation < 1 || parcours.AnneeFormation > 2)
        {
            throw new FormationYearExecption.FormationYearException("L'année de formation est incorrecte.");
        }
    }
}