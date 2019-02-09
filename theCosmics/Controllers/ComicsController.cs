using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using theCosmics.Models;

namespace theCosmics.Controllers
{
    public class ComicsController : Controller
    {
        private ProductContext db = new ProductContext();

        //
        // GET: /Comics/

        public ActionResult Index(int? skip)
        {
            // Set the Current Category Page
            ViewBag.CurrentPage = "Comics";

            // Pass SEO meta tag
            ViewBag.Description = "A collection of awesome gift ideas in " + ViewBag.CurrentPage;
            ViewBag.Keywords = "Gift ideas in" + ViewBag.CurrentPage;

            // Filter Products by Category
            IEnumerable<Product> products = db.Products.Where(prod => prod.Category == "Comics");

            // Count the number of Products and pass to the View
            ViewBag.ProductCount = products.Count();

            // Sort Products by Hits
            products = products.OrderByDescending(i => i.Hits);

            // Skip results already viewed
            if ((skip != null) && (skip > 0))
            {
                products = products.Skip((int)skip);
                ViewBag.Skip = (int)skip;
            }
            else
            {
                ViewBag.Skip = 0;
            }

            // Get 9 products
            products = products.Take(9);

            return View(products);
        }

    }
}
