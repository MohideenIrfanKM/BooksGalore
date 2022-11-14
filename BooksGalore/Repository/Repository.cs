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

        IEnumerable<T> IRepository<T>.GetAll()
        {
            IQueryable<T> query = dbset;//////////////////WHY????? create Intrface obj
            return query.ToList();
        }

        T IRepository<T>.getFirstorDefault(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbset;
            query = query.Where(filter);
            return query.FirstOrDefault();

        }

        void IRepository<T>.Remove(T entity)
        {
            dbset.Remove(entity);
        }
    }
}
