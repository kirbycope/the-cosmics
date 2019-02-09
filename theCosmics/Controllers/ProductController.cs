using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using theCosmics.Models;

namespace theCosmics.Controllers
{
    public class ProductController : Controller
    {
        private ProductContext db = new ProductContext();

        //
        // GET: /Product/
        
        public ActionResult Index()
        {
            ViewBag.CurrentPage = "Products";

            // Pass SEO meta tag
            ViewBag.Description = "A collection of awesome gift ideas from tech geeks.";
            ViewBag.Keywords = "Gift idea categories from theCosmics";

            return View();
        }

        //
        // GET: /Product/Featured

        public IEnumerable<Product> Featured()
        {
            // Get Featured products
            IEnumerable<Product> products = db.Products.Where(prod => prod.Featured.Equals(true));

            // Order by Category
            products = products.OrderByDescending(i => i.Category);

            return products;
        }

        //
        // GET: /Product/TopProducts
        public IEnumerable<Product> TopProducts()
        {
            // Get the Products
            IEnumerable<Product> products = db.Products.ToList();

            // Order by Hits
            products = products.OrderByDescending(p => p.Hits);

            // Limit to 5
            products = products.Take(5);

            return products;
        }

        //
        // GET: /Product/TopProductsByCategory
        public IEnumerable<Product> TopProductsByCategory(string category)
        {
            // Get the Products
            IEnumerable<Product> products = db.Products.Where(prod => prod.Category == category);

            // Order by Hits
            products = products.OrderByDescending(p => p.Hits);

            // Limit to 5
            products = products.Take(4);

            return products;
        }

        //
        // GET: /Product/Details/5

        public ActionResult Details(int id = 0)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Increment the product view (Hits) counter
            product.Hits++;
            db.SaveChanges();

            // Get the product's category to highlight on nav bar
            ViewBag.CurrentPage = product.Category.ToLower();

            // Pass the product's category (Proper formatting)
            ViewBag.Category = product.Category;

            // Pass SEO meta tag
            ViewBag.Description = product.ProductHeader;
            ViewBag.Keywords = product.ProductName + " in " + product.Category;

            return View(product);
        }

        //
        // GET: /Product/Create
        [Authorize]
        public ActionResult Create(string Category)
        {
            ViewBag.CurrentPage = "Create New Product";

            if (Category != null)
            {
                ViewBag.Category = Category;
            }
            else
            {
                ViewBag.Category = "All";
            }

            return View();
        }

        //
        // POST: /Product/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    product.ImageMimeType = image.ContentType;
                    product.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(product.ImageData, 0, image.ContentLength);
                }

                product.Hits = 0;
                db.Products.Add(product);
                db.SaveChanges();

                // Send user back to the appropriate admin
                return RedirectToAction("List", new { Category = product.Category });
            }

            return View(product);
        }

        //
        // GET: /Product/GetImage/5

        public FileContentResult GetImage(int id)
        {
            Product product = db.Products.Find(id);
            if((product.ImageData != null) && (product.ImageMimeType != null))
            {
                return File(product.ImageData, product.ImageMimeType);
            }
            else
            {
                return null;
            }
        }

        //
        // GET: /Product/Edit/5
        [Authorize]
        public ActionResult Edit(int id = 0)
        {
            ViewBag.CurrentPage = "Edit Product";

            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        //
        // POST: /Product/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    product.ImageMimeType = image.ContentType;
                    product.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(product.ImageData, 0, image.ContentLength);
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();

                // Send user back to the appropriate admin
                return RedirectToAction("List", new { Category = product.Category });
            }
            return View(product);
        }

        //
        // GET: /Product/Delete/5
        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            ViewBag.CurrentPage = "Delete Product";

            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        //
        // POST: /Product/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();

            // Send user back to the appropriate admin
            return RedirectToAction("List", new { Category = product.Category });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}