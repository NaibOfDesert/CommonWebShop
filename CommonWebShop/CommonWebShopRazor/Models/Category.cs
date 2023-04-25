using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CommonWebShopRazor.Models
{
    public class Category
    {
        [Key] // primaty key
        public int Id { get; set; } // primary key can by set without [Key] by adding CategoryId    
        [Required]
        [MaxLength(10, ErrorMessage = "Invalid Name")]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Invalid Order, must be between 1-100")]
        public int DisplayOrder { get; set; }
    }
}
