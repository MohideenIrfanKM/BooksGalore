using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository.IRepository;

namespace BooksGalore.Repository
{
    public class UnitofWork : IUnitofWork
    {
        BooksGalore.Db.Dbcontext db;
        public UnitofWork(Dbcontext db)
        {
            this.db = db;
            CategoryRepository = new CategoryRepository(db);//this way we can access repository as DbContext
            CoverTypeRepository = new CoverTypeRepository(db);
            ProductRepository = new ProductRepository(db);
            CompanyRepository = new CompanyRepository(db);
            ApplicationUserRepository = new ApplicationUserRepository(db);
            ShoppingCartRepository = new ShoppingCartRepository(db);

        }
        public ICategoryRepository CategoryRepository { get; private set; }
        public ICoverTypeRepository CoverTypeRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public ICompanyRepository CompanyRepository { get; private set; }
        public IApplicationUserRepository ApplicationUserRepository { get; set; }
        public IShoppingCartRepository ShoppingCartRepository { get; set; }


        public void Save()
        {
            db.SaveChanges();
        }
    }
}
