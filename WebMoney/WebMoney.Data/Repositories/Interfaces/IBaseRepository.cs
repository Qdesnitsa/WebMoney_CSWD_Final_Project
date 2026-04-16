using WebMoney.Persistence.Entities;

namespace WebMoney.Data.Repositories.Interfaces;

public interface IBaseRepository<T> where T : BaseEntity
{
    void Create(T entity);
    T GetById(int id);
}