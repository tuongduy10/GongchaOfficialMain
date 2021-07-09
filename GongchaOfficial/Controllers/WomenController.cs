using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GongchaOfficial.Models;

namespace GongchaOfficial.Controllers
{
    public class WomenController : Controller
    {
        dbGongchaDataContext data = new dbGongchaDataContext();

        // GET: Women
        private List<Product> getNewProDuctWithSex(int count)
        {
            return data.Products.Where(s => s.SexId == "women" || s.SexId == "unisex").OrderByDescending(a => a.ProductReleaseDate).Take(count).ToList();
        }
        private List<Product> getProductWithId(String id)
        {
            return data.Products.Where(s => s.CategoryId == id).ToList();
        } 
        public ActionResult Index(String id)
        {
            if (id == null)
            {
                var product = getNewProDuctWithSex(20);
                return View(product);
            }
            else
            {
                var product = data.Products.Where(s => s.CategoryId == id).ToList();
                return View(product);
            }
        }

        //Load Category to left list
        public ActionResult Category()
        {
            var listcategory = from ct in data.Categories.Where(a => a.SexId == "women" || a.SexId =="unisex") select ct;
            return PartialView(listcategory);
        }

        public ActionResult ProductDetail(string id)
        {
            var product = from pd in data.Products
                          where pd.ProductId == id
                          select pd;

            return View(product.Single());
        }

    }
}