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
	public class OrderHeader
	{
		[Key]
		public int Id { get; set; }

		public string ApplicationUserId {  get; set; }
		[ForeignKey(nameof(ApplicationUserId))]
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }

		[Required] 
		public DateTime OrderDate {  get; set; }

		public DateTime ShippingDate {  get; set; }

		public double OrderTotal { get; set; }	

		public string? OrderStatus {  get; set; }

		public string? PaymentStatus {  get; set; }

		public string? TrackingNumber {  get; set; }

		public string? Carrier {  get; set; }

		public DateTime PaymentDate { get; set; }

		public DateTime PaymentDueDate { get; set; }

		//for stripe payment
		public string? SessionId {  get; set; }

		public string? PaymentId { get; set; }

		[Required]
		public string PhoneNumber { get; set; }
		[Required]
		public string StreetAddress { get; set; }
		[Required]
		public string City { get; set; }
		[Required]
		public string State { get; set; }
		[Required]
		public string PostalCode { get; set; }
		[Required]
		public string Name { get; set; }
	}
}
