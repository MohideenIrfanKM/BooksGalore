﻿using BooksGalore.Db;
using BooksGalore.Models;
using BooksGalore.Repository.IRepository;
using BooksGalore.Utility;

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
			

            if (obj != null)
			{
				obj.OrderStatus = OrderStatus;
				//db.SaveChanges(); this can be done in controller level using unitofwork
			}
			if(PaymentStatus!=null)
			{
				if(PaymentStatus==Util.PaymentStatusApproved)
				{
                    obj.PaymentDate = DateTime.Now;

                }
                obj.PaymentStatus=PaymentStatus; 
				//db.SaveChanges();
			}
		}

		public void UpdateStripePaymentId(int id, string SessionId, string PaymentId)
		{
		   OrderHeader orderHeader = db.OrderHeaders.FirstOrDefault(u=>u.Id == id);
			if(orderHeader != null)
			{

                orderHeader.SessionId=SessionId;
				orderHeader.PaymentId=PaymentId;
				
			}
		}
	}
}
