using System.Linq.Expressions;

namespace BooksGalore.Repository.IRepository
{
    public interface IRepository<T> where T:class
    {
        IEnumerable<T> GetAll(System.Linq.Expressions.Expression<Func<T,bool>>? filter=null, string? includeProperties=null);
        void Add(T entity); 
        void Remove(T entity);
		void RemoveRange(IEnumerable<T> entity);

		T getFirstorDefault(Expression<Func<T, bool>> filter,string? includeProperties=null);
    }
}
