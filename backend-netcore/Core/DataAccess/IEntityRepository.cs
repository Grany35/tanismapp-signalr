using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataAccess
{
    public interface IEntityRepository<T> where T : class, new()
    {
        Task Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        Task<List<T>> GetAllAsnc(Expression<Func<T, bool>> filter = null);
        Task<T> GetAsnc(Expression<Func<T, bool>> filter);
        T Get(Expression<Func<T, bool>> filter);
    }
}
