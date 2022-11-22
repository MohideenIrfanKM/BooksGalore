
using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository.IRepository;

namespace BooksGalore.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository

    {

        private readonly Dbcontext db;

        public CategoryRepository(Dbcontext db):base(db)
        {
                this.db = db;
        }
        public void Update(Category category)
        {
            db.Categories.Update(category);
            

        }
    }
}
