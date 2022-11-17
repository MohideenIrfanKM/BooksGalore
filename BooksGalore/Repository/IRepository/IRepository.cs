using System.Linq.Expressions;

namespace BooksGalore.Repository.IRepository
{
    public interface IRepository<T> where T:class
    {
        IEnumerable<T> GetAll(string? includeProperties=null);
        void Add(T entity); 
        void Remove(T entity);
        T getFirstorDefault(Expression<Func<T, bool>> filter,string? includeProperties=null);
    }
}
