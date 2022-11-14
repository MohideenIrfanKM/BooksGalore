using System.ComponentModel.DataAnnotations;

namespace BooksGalore.Models
{
    public class Category
    {
        [Key]
        public  int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required(ErrorMessage ="The Display Order field required")]
        [Range(1,100,ErrorMessage ="choose between 1-100")]
        public int DisplayOrder { get; set; }
        public DateTime CreatedDate { get; set; }= DateTime.Now;    
    }
}
