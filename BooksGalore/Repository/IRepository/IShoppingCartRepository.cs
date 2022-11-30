using BooksGalore.Models;

namespace BooksGalore.Repository.IRepository
{
	public interface IShoppingCartRepository:IRepository<ShoppingCart>
	{
		public int IncCount(ShoppingCart cart,int count);
		public int DecCount(ShoppingCart cart,int count);
	}
}
