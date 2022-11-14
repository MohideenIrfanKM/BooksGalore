using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository.IRepository;

namespace BooksGalore.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        Dbcontext db;
        public CoverTypeRepository(Dbcontext db) : base(db)
        {
            this.db = db;
        }

        public void Update(CoverType cvr)
        {
            db.CoverTypes.Update(cvr);
        }
    }
}
