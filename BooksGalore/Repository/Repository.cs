using BooksGalore.Db;
using BooksGalore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BooksGalore.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly Dbcontext db;
        internal DbSet<T> dbset;
        public Repository(Dbcontext db)
        {
            this.db = db;
            this.dbset = db.Set<T>();
        }

        void IRepository<T>.Add(T entity)
        {
            dbset.Add(entity);
        }

        IEnumerable<T> IRepository<T>.GetAll(System.Linq.Expressions.Expression<Func<T,bool>>? filter=null, string? includeProperties=null)
        {
            IQueryable<T> query = dbset;
             //////////////////WHY????? create Intrface obj
             if(filter!=null)
              query = query.Where(filter);
            if(includeProperties!=null)
                foreach(var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)){
                    query=query.Include(property);
                }
            return query.ToList();
        }

        T IRepository<T>.getFirstorDefault(System.Linq.Expressions.Expression<Func<T, bool>> filter, string? includeProperties=null)
        {
            IQueryable<T> query = dbset;
            query = query.Where(filter);
            if (includeProperties != null)
                foreach (var property in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query=query.Include(property);
                }
            return query.FirstOrDefault();

        }

        void IRepository<T>.Remove(T entity)
        {
            dbset.Remove(entity);
        }
		public void RemoveRange(IEnumerable<T> entity)
		{
			dbset.RemoveRange(entity);
		}
	}
}
