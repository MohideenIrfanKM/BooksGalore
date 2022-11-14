
using BooksGalore.Models;

namespace BooksGalore.Repository.IRepository
{
    public interface IProductRepository:IRepository<Product>
    {
        public void Update(Product product);
    }
}
