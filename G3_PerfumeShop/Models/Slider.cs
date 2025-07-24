using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace G3_PerfumeShop.Models
{
    public class Slider
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? ImagePath { get; set; }

        public bool Status { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }


}
