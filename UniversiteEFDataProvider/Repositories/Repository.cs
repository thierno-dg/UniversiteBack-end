using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteEFDataProvider.Data;


public abstract class Repository<T>(UniversiteDbContext context) : IRepository<T>
    where T : class
{
    protected  readonly UniversiteDbContext Context = context;
    
    public async Task<T> CreateAsync(T entity)
    {
        var res = Context.Add(entity);
        await Context.SaveChangesAsync();
        return res.Entity;
    }
    public async Task UpdateAsync(T entity)
    {
        var res=Context.Set<T>().Update(entity);
        await Context.SaveChangesAsync();
    }
    public virtual async Task DeleteAsync(long id)
    {
        var entity = await FindAsync(id);
        if (entity != null)
        {
            try
            {
                Context.Remove(entity);
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

   
    
    public async Task DeleteAsync(T entity)
    {
        Context.Remove(entity);
        await Context.SaveChangesAsync();
    }
    
    // Clé primaire non composée
    public async Task<T?> FindAsync(long id)
    {
        return await Context.Set<T>().FindAsync(id);
    }
    // Clé primaire composée
    public async Task<T?> FindAsync(params object[] keyValues)
    {
        return await Context.Set<T>().FindAsync(keyValues);
    }
    
    public async Task<T> AddUeAsync(long idParcours, long[] idUes)
    {
        var parcours = await Context.Set<T>().FindAsync(idParcours);
        if (parcours == null)
        {
            throw new ArgumentException($"Aucun parcours trouvé avec l'ID {idParcours}");
        }
        await Context.SaveChangesAsync();
        return parcours;
    }

    
    public async Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> condition)
    {
        return await Context.Set<T>().Where(condition).ToListAsync();
    }

    public async Task<List<T>> FindAllAsync()
    {
        return await Context.Set<T>().ToListAsync();
    }

    public Task SaveChangesAsync()
    {
        return Context.SaveChangesAsync();
    }
}