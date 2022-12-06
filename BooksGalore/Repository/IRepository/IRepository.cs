using System.Linq.Expressions;

namespace BooksGalore.Repository.IRepository
{
    public interface IRepository<T> where T:class
    {
        IEnumerable<T> GetAll(System.Linq.Expressions.Expression<Func<T,bool>>? filter=null, string? includeProperties=null);
        void Add(T entity); 
        void Remove(T entity);
		void RemoveRange(IEnumerable<T> entity);

		T getFirstorDefault(Expression<Func<T, bool>> filter,string? includeProperties=null, bool? tracked=true);
    //we are using Expression here because it contains the structure of its contents which is must in order to work with sql filtering
    }

}
