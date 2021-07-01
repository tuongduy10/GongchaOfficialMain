using GongchaOfficial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GongchaOfficial.Controllers
{
    public class DetailController : Controller
    {
        dbGongchaDataContext data = new dbGongchaDataContext();
        // GET: Detail
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ProductDetail(string id)
        {
            var product = from pd in data.Products where pd.ProductId == id select pd;
            return View(product.Single());
        }
    }
}