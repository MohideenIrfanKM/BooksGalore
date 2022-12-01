using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
//using MessagePack;

namespace BooksGalore.Models
{
    public class ShoppingCart //problem come only on post if we didn't fill all the fields not in GET
    {
        [Key]
        public int Id { get; set; }

        public int ProductId {  get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product product { get; set; }

        public string ApplicationUserId {  get; set; }
        [ValidateNever]
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; } //press appuser f12 to see the implement of class
        [Required]
        //[Range(1,1000,ErrorMessage ="Count value should be within 1 - 1000 ")]
		[Range(1, 10000, ErrorMessage = "choose between 1-100")]
		public int count { get; set; }
        [NotMapped]
        [ValidateNever]
        public double price { get; set; }
    }
}
