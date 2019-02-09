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
using System.Text.RegularExpressions;

namespace theCosmics.Controllers
{
    public class BlogController : Controller
    {
        private BlogContext db = new BlogContext();

        //
        // GET: /Blog/
        
        public ActionResult Index(string Category, int?skip)
        {
            // Set the Current Page
            ViewBag.CurrentPage = "Blog";

            IEnumerable<Blog> blogs = null;

            if ((Category != null) && (Category != "All"))
            {
                // Filter Blog post by Category
                blogs = db.Blogs.Where(blogPost => blogPost.Category == Category);
                ViewBag.Category = Category;
            }
            else
            {
                // Send back all Blog posts
                blogs = db.Blogs.ToList();
                ViewBag.Category = "All";
            }

            // Pass SEO meta tag
            ViewBag.Description = "Cosmic Blog category " + ViewBag.Category;
            ViewBag.Keywords = "Cosmic Blog, Cosmic Blog categories, theCosmics Blog, theCosmic Blog, " + ViewBag.Category;

            // Order Blogs by Date, Newest to Oldest
            blogs = blogs.OrderByDescending(post => post.Created);

            // Count the number of posts and pass to the View
            ViewBag.PostCount = blogs.Count();

            // Skip results already viewed
            if ((skip != null) && (skip > 0))
            {
                blogs = blogs.Skip((int)skip);
                ViewBag.Skip = (int)skip;
            }
            else
            {
                ViewBag.Skip = 0;
            }

            // Get 9 posts
            blogs = blogs.Take(9);

            // Remove html formatting from the Body of the post(s)
            foreach (var blog in blogs)
            {
                blog.Body = Regex.Replace(blog.Body, "<.+?>", string.Empty);
            }

            return View(blogs);
        }

        //
        // GET: /Blog/Categories
        public IEnumerable<string> Categories()
        {
            // Get a list of Categories
            IEnumerable<string> categories = db.Blogs.Select(post => post.Category).Distinct();

            return categories;
        }

        //
        // GET: /Blog/LatestPosts
        public IEnumerable<Blog> LatestPosts()
        {
            // Get blog posts
            IEnumerable<Blog> posts = db.Blogs.ToList();

            // Sort posts by hits
            posts = posts.OrderByDescending(post => post.Created);

            // Trim to 6 posts
            posts = posts.Take(6);

            // Remove html formatting from the Body of the post(s)
            foreach(var post in posts)
            {
                post.Body = Regex.Replace(post.Body, "<.+?>", string.Empty);
            }

            return posts;
        }

        //
        // GET: /Blog/TopBlogCategories
        public IEnumerable<TopCategoryModel> TopBlogCategories()
        {
            return db.Blogs
                .Select(b => b.Category)
                .Distinct()
                .Select(c => new TopCategoryModel
                {
                    Category = c,
                    Count = db.Blogs.Count(b => b.Category == c)
                })
                .OrderByDescending(a => a.Count)
                .Take(5);
        }

        //
        // GET: /Blog/GetBioPic

        public static string GetBioPic(string Author)
        {
            string BioPicURL = null;

            if (Author == "Tim")
            {
                BioPicURL = "/Images/tim.png";
            }
            else if (Author == "Jess")
            {
                BioPicURL = "/Images/jess.png";
            }
            else
            {
                BioPicURL = "/Images/warning-512.png";
            }

            return BioPicURL;
        }

        //
        // GET: /Blog/GetTopBlogPost

        public Blog GetTopBlogPost(string Author)
        {
            // Get the most popular blog post for passed in Author
            Blog BlogPost = (from blog in db.Blogs
                             where blog.Author == Author
                             orderby blog.Hits descending
                             select blog).First();

            // Remove html formatting from the Body of the post
            BlogPost.Body = Regex.Replace(BlogPost.Body, "<.+?>", string.Empty);

            return BlogPost;
        }

        //
        // GET: /Blog/Blogger

        public ActionResult Blogger()
        {
            ViewBag.CurrentPage = "Blog";

            // Pass SEO meta tag
            ViewBag.Description = "Cosmic Blog Writers Profile Page";
            ViewBag.Keywords = "theCosmic Blog writers, Cosmic Blog writers, theCosmic Blog blogger, Cosmic Blog blogger";

            // Get the Categories that "Jess" has posted under
            IEnumerable<string> jessCategories = (from blog in db.Blogs
                                                  where blog.Author == "Jess"
                                                  select blog.Category).Distinct();

            // Get the Categories that "Tim" has posted under
            IEnumerable<string> timCategories = (from blog in db.Blogs
                                                  where blog.Author == "Tim"
                                                  select blog.Category).Distinct();

            Tuple<IEnumerable<string>, IEnumerable<string>> bloggers = new Tuple<IEnumerable<string>, IEnumerable<string>>(jessCategories, timCategories);

            return View(bloggers);
        }

        //
        // GET: /Blog/Post/5
        
        public ActionResult Post(int id = 0)
        {
            // Set the Current Page
            ViewBag.CurrentPage = "Blog";

            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }

            // Increment the product view (Hits) counter
            blog.Hits++;
            db.SaveChanges();

            // Pass SEO meta tag
            ViewBag.Description = Regex.Replace(blog.Body, "<.+?>", string.Empty);

            return View(blog);
        }

        //
        // GET: /Blog/Create
        [Authorize]
        public ActionResult Create(string Category)
        {
            // Set the Current Page
            ViewBag.CurrentPage = "Blog";

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
        // POST: /Blog/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Blog blog)
        {
            if (ModelState.IsValid)
            {
                // Save DateTime
                blog.Created = DateTime.Now;
                // Set default for Hits
                blog.Hits = 0;

                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blog);
        }

        //
        // GET: /Blog/Edit/5
        [Authorize]
        public ActionResult Edit(int id = 0)
        {
            // Set the Current Page
            ViewBag.CurrentPage = "Blog";

            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        //
        // POST: /Blog/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Blog blog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        //
        // GET: /Blog/Delete/5
        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            // Set the Current Page
            ViewBag.CurrentPage = "Blog";

            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        //
        // POST: /Blog/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Blog blog = db.Blogs.Find(id);
            db.Blogs.Remove(blog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}