using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace theCosmics.Models
{
    public class Product
    {
        // Product Details
        public int ID { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string ProductHeader { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string LongDescription { get; set; }
        [Required]
        public string TrackingURL { get; set; }

        // Image
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }

        // Page Hit Tracking
        public int Hits { get; set; }

        // Featured Flag
        public bool Featured { get; set; }
    }
}