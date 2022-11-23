using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository.IRepository;

namespace BooksGalore.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly Dbcontext db;

        public CompanyRepository(Dbcontext db) : base(db)
        {
            this.db = db;
        }

        public void Update(Company cmp)
        {
            db.Companies.Update(cmp);
        }
    }
}
