using BooksGalore.Models;

namespace BooksGalore.Repository.IRepository
{
	public interface IOrderHeaderRepository : IRepository<OrderHeader>
	{
		public void Update(OrderHeader orderHeader);

		public void UpdateStatus(int id, string OrderStatus, string? PaymentStatus);
		//as payment can be done after 30d for same users, it may be null

		public void UpdateStripePaymentId(int id, string SessionId, string PaymentId);
		
	}
}
