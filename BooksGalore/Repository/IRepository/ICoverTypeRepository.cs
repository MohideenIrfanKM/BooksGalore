using BooksGalore.Models;

namespace BooksGalore.Repository.IRepository
{
    public interface ICoverTypeRepository : IRepository<CoverType>
    {
        public void Update(CoverType cvr);
    }
}
