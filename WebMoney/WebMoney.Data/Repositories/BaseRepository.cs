using Microsoft.EntityFrameworkCore;
using WebMoney.Data.Repositories.Interfaces;
using WebMoney.Data.Entities;

namespace WebMoney.Data.Repositories;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected WebContext webContext;
    protected DbSet<TEntity> dbSet;

    public BaseRepository(WebContext webContext)
    {
        this.webContext = webContext;
        dbSet = webContext.Set<TEntity>();
    }

    public virtual void Create(TEntity entity)
    {
        dbSet.Add(entity);
        webContext.SaveChanges();
    }

    public virtual TEntity? GetById(int id) => dbSet.Find(id);
    public void SaveChanges() => webContext.SaveChanges();
}