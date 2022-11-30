using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository.IRepository;

namespace BooksGalore.Repository
{
	public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
	{
		public readonly Dbcontext db;
		public ShoppingCartRepository(Dbcontext db) : base(db)
		{
			this.db = db;
		}

		public int DecCount(ShoppingCart cart,int count)
		{
			cart.count -= count;
			return count;
		}

		public int IncCount(ShoppingCart cart,int count)
		{
			cart.count += count;
			return count;
		}
	}
}
