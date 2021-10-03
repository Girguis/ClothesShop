using System.Collections.Generic;

namespace BLL
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetByID(long id);
        T Add(T obj);
        bool Update(T obj);
        bool Delete(long id);


    }
}
