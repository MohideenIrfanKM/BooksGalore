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
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }   
        public string Description { get; set; }
        public string ISBN { get; set; }
        public string Author { get; set; }
        [Required]
       
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        [Required]
        public int CoverTypeId { get; set; }
        [ForeignKey("CoverTypeId")]
        [ValidateNever]
        public CoverType covertype { get; set; }
        [Range(1,10000)]
        public double listprice { get; set; }
        [Range(1,10000)]
        public double price { get; set; }
        [Range(1, 10000)]
        public double price50 { get; set; }
        [Range(1,10000)]
        public double price100 { get; set; }  
        [ValidateNever]
        public string ImageURL { get; set; }


    }
}
