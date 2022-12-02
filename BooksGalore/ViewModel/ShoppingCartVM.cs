using System.ComponentModel.DataAnnotations.Schema;
using BooksGalore.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing.Constraints;

namespace BooksGalore.ViewModel
{
	public class ShoppingCartVM
	{
		public List<ShoppingCart> scart {  get; set; }
	
		public OrderHeader orderHeader { get; set; }

	}
}
