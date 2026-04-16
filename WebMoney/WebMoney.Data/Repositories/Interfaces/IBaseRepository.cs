using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Persistence;

public interface IBaseRepository<T> where T : BaseEntity
{
    void Create(T entity);
    T GetById(int id);
}