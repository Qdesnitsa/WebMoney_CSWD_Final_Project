using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Repositories.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    void Create(TEntity entity);
    TEntity GetById(int id);
}