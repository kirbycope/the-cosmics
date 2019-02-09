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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.CurrentPage = "Home";

            // Pass SEO meta tag
            ViewBag.Description = "A collection of awesome gift ideas and blogs from tech geeks.";
            ViewBag.Keywords = "gift ideas, tech gifts, geeky gifts, gifts for him, gifts for her, gifts under $20, tech blog, apparel, comics, gadgets, games, movies";

            return View();
        }
    }
}
