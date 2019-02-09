using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace theCosmics.Models
{
    public class Blog
    {
        // Blog Post Details
        public int ID { get; set; }
        public DateTime Created { get; set; }
        public int Hits { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Category {get; set; }
        [Required]
        [UIHint("tinymce_jquery_full"), AllowHtml]
        public string Body {get; set;}
    }
}