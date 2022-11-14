using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository.IRepository;

namespace BooksGalore.Repository
{
    public class UnitofWork : IUnitofWork
    {
        Dbcontext db;
        public UnitofWork(Dbcontext db)
        {
            this.db = db;
            CategoryRepository = new CategoryRepository(db);
            CoverTypeRepository = new CoverTypeRepository(db);
        }
        public ICategoryRepository CategoryRepository { get; private set; }
        public ICoverTypeRepository CoverTypeRepository { get; private set; }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}
