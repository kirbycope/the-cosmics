using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using theCosmics.Filters;
using theCosmics.Models;

namespace theCosmics.Controllers
{
    public class AdminController : Controller
    {
        private BlogContext db1 = new BlogContext();
        private ProductContext db2 = new ProductContext();

        //
        // GET: /Admin/
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.CurrentPage = "Admin";

            return View();
        }

        //
        // GET: /Admin/Blog
        [Authorize]
        public ActionResult Blog(string Category)
        {
            ViewBag.CurrentPage = "Blog Admin";

            IEnumerable<Blog> blogs = null;

            if ((Category != null) && (Category != "All"))
            {
                // Filter Blog post by Category
                blogs = db1.Blogs.Where(blogPost => blogPost.Category == Category);
                ViewBag.Category = Category;
            }
            else
            {
                // Send back all Blog posts
                blogs = db1.Blogs.ToList();
                ViewBag.Category = "All";
            }

            // Get a list of Categories
            IEnumerable<string> categories = db1.Blogs.Select(post => post.Category).Distinct();

            // Save the list of posts and list of categories to a tuple
            Tuple<IEnumerable<Blog>, IEnumerable<string>> tuple = new Tuple<IEnumerable<Blog>, IEnumerable<string>>
                                                                  (blogs, categories);

            return View(tuple);
        }

        //
        // GET: /Admin/Product
        [Authorize]
        public ActionResult Product(string Category)
        {
            ViewBag.CurrentPage = "Product Admin";

            IEnumerable<Product> products = null;

            if ((Category != null) && (Category != "All"))
            {
                // Filter Products by Category
                products = db2.Products.Where(prod => prod.Category == Category);
                ViewBag.Category = Category;
            }
            else
            {
                // Send back all Products
                products = db2.Products.ToList();
                ViewBag.Category = "All";
            }

            // Get a list of Categories
            IEnumerable<string> categories = db2.Products.Select(prod => prod.Category).Distinct();

            // Save the list of blogs and list of categories to a tuple
            Tuple<IEnumerable<Product>, IEnumerable<string>> tuple = new Tuple<IEnumerable<Product>, IEnumerable<string>>
                                                                 (products, categories);

            return View(tuple);
        }

        protected override void Dispose(bool disposing)
        {
            db1.Dispose();
            db2.Dispose();
            base.Dispose(disposing);
        }

    }

}
