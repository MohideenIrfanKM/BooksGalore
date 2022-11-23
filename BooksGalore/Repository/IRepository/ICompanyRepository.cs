using BooksGalore.Models;

namespace BooksGalore.Repository.IRepository
{
    public interface ICompanyRepository:IRepository<Company>
    {
        public void Update(Company cmp);

    }
}
