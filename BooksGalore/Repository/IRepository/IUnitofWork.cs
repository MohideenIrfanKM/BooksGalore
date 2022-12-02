using BooksGalore.Db;
using BooksGalore.Models;

namespace BooksGalore.Repository.IRepository
{
    public interface IUnitofWork
    {
        
        ICategoryRepository CategoryRepository { get;  }
        ICoverTypeRepository CoverTypeRepository { get; }
        IProductRepository ProductRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        IShoppingCartRepository ShoppingCartRepository { get; }
        IApplicationUserRepository ApplicationUserRepository { get; }
	    IOrderHeaderRepository OrderHeaderRepository { get; set; }
	    IOrderDetailsRepository OrderDetailsRepository { get; set; }
		public void Save();
    }
}
