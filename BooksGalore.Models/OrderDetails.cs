using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BooksGalore.Models
{
	public class OrderDetails
	{
		[Key]
		public int Id { get; set; }

		public int ProductId { get; set; }

		[ForeignKey("ProductId")]
		[ValidateNever]
		public Product product { get; set; }

		public int OrderId { get; set; }

		[ForeignKey("OrderId")]
		[ValidateNever]
		public OrderHeader OrderHeader { get; set; }

		public int Count { get; set; }

		public double Price { get; set; }
	}
	
}
