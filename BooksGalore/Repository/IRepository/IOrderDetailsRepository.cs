using BooksGalore.Models;

namespace BooksGalore.Repository.IRepository
{
	public interface IOrderDetailsRepository : IRepository<OrderDetails>
	{
		public void Update(OrderDetails orderDetails);
	}
}
