using BooksGalore.Db;
using BooksGalore.Models;

namespace BooksGalore.Repository.IRepository
{
    public interface IUnitofWork
    {
        
        ICategoryRepository CategoryRepository { get;  }
        ICoverTypeRepository CoverTypeRepository { get; }

        public void Save();
    }
}
