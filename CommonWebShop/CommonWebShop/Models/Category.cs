using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CommonWebShop.Models
{
    public class Category
    {
        [Key] // primaty key
        public int Id { get; set; } // primary key can by set without [Key] by adding CategoryId    
        [Required]
        [MaxLength(30, ErrorMessage = " Wrogn Name")]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Wrong Order")]
        public int DisplayOrder { get; set; }
    }
}
