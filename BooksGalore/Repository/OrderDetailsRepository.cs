using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository.IRepository;

namespace BooksGalore.Repository
{
	public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
	{
		private readonly Dbcontext db;
		public OrderDetailsRepository(Dbcontext db) : base(db)
		{
			this.db = db;
		}

		public void Update(OrderDetails orderDetails)
		{
			db.OrderDetails.Update(orderDetails);
		}
	}
}
