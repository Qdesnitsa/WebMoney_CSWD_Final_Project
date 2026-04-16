using Microsoft.EntityFrameworkCore;
using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Persistence;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected WebContext webContext;
    protected DbSet<T> dbSet;

    public BaseRepository(WebContext webContext)
    {
        this.webContext = webContext;
        dbSet = webContext.Set<T>();
    }

    public void Create(T entity)
    {
        dbSet.Add(entity);
        webContext.SaveChanges();
    }

    public T? GetById(int id) => dbSet.Find(id);
}