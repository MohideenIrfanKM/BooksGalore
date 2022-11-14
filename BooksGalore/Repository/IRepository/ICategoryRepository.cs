using BooksGalore.Models;

namespace BooksGalore.Repository.IRepository
{
    public interface ICategoryRepository:IRepository<Category>
    {
        public void Update(Category category);
    }
}
