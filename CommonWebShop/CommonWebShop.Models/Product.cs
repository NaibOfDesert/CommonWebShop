﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
        [DisplayName("Price for 1-10")]
        [Range(1, 100000)]
        public double Price { get; set; }
        [Required]
        [DisplayName("Price for 10-..")]
        [Range(1, 100000)]
        public double Price10 { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        // TODO: add tom
        // TODO: add title as class









    }
}
