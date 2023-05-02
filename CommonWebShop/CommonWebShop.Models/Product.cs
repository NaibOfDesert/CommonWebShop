﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonWebShop.Models
{
    public class Product
    {
        [Key] // primaty key
        public int Id { get; set; } // primary key can by set without [Key] by adding CategoryId    
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Display(Name = "List Price")]
        [Range(1, 100000)]
        public double ListPrice { get; set; }
        [Required]
        [Display(Name = "Price for 0-10")]
        [Range(1, 100000)]
        public double Price { get; set; }
        [Required]
        [Display(Name = "Price for 10-..")]
        [Range(1, 100000)]
        public double Price10 { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Description { get; set; }
        Category? Category { get; set; }







    }
}