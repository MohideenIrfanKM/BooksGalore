using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository.IRepository;

namespace BooksGalore.Repository
{
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly Dbcontext db;
		public OrderHeaderRepository(Dbcontext db) : base(db)
		{
			this.db = db;
		}

		public void Update(OrderHeader orderHeader)
		{
			db.OrderHeaders.Update(orderHeader);
		}

		public void UpdateStatus(int id, string OrderStatus, string? PaymentStatus=null)
		{
			var obj=db.OrderHeaders.FirstOrDefault(o => o.Id == id);	
			if(obj != null)
			{
				obj.OrderStatus = OrderStatus;
				//db.SaveChanges(); this can be done in controller level using unitofwork
			}
			if(PaymentStatus!=null)
			{
				obj.PaymentStatus=PaymentStatus; 
				//db.SaveChanges();
			}
		}
	}
}
