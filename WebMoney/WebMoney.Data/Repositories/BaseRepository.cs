using Microsoft.EntityFrameworkCore;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Repositories;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected WebContext webContext;
    protected DbSet<T> dbSet;

    public BaseRepository(WebContext webContext)
    {
        this.webContext = webContext;
        dbSet = webContext.Set<T>();
    }

    public virtual void Create(T entity)
    {
        dbSet.Add(entity);
        webContext.SaveChanges();
    }

    public virtual T? GetById(int id) => dbSet.Find(id);
}