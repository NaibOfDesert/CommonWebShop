using System.ComponentModel.DataAnnotations;

namespace CommonWebShop.Models
{
    public class Category
    {
        [Key] // primaty key
        public int Id { get; set; } // primary key can by set without [Key] by adding CategoryId    
        [Required]
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
