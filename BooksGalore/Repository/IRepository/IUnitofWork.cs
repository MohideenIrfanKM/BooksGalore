using BooksGalore.Db;
using BooksGalore.Models;

namespace BooksGalore.Repository.IRepository
{
    public interface IUnitofWork
    {
        
        ICategoryRepository CategoryRepository { get;  }
        ICoverTypeRepository CoverTypeRepository { get; }
        IProductRepository ProductRepository { get; }

        public void Save();
    }
}
