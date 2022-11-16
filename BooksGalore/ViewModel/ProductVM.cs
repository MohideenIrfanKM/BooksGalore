using BooksGalore.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BooksGalore.ViewModel
{
    public class ProductVM
    {
        public Product product { get; set; }


        [ValidateNever]
        public IEnumerable<SelectListItem> categlist { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> coverlist { get; set; }


    }
}
