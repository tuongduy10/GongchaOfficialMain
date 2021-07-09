using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GongchaOfficial.Models;

namespace GongchaOfficial.Controllers
{
    public class CartController : Controller
    {
        dbGongchaDataContext db = new dbGongchaDataContext();

        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }

        public List<dataCart> ListCart()
        {
            List<dataCart> cart = Session["Cart"] as List<dataCart>;
            if (cart == null)//Kiểm tra GIỎ HÀNG 
            {
                cart = new List<dataCart>();
                Session["Cart"] = cart;
            }
            return cart;
        }
        public ActionResult AddToCart(string ProductId, string url)
        {
            List<dataCart> listInCart = ListCart();
            dataCart pd = listInCart.Find(p => p.ProductId == ProductId);
        
            if (pd == null)//Kiểm tra SẢN PHẨM trong GIỎ HÀNG
            {        
                pd = new dataCart(ProductId);//Tạo mới 1 sản phẩm 
                listInCart.Add(pd);//Thêm sản phẩm vào giỏ hàng
                return Redirect(url);
            }
            else
            {
                pd.Amount++;
                return Redirect(url);
            }
        }
        public ActionResult UpdateFromCart(String ProductId, FormCollection frm)
        {
            List<dataCart> listCart = ListCart();
            dataCart pd = listCart.SingleOrDefault(p => p.ProductId == ProductId);
            if (pd != null)
            {
                pd.Amount = int.Parse(frm["txtAmount"].ToString());
            }

            return RedirectToAction("Cart");
        }
        public ActionResult RemoveFromCart(string ProductId, string url)
        {
            List<dataCart> listInCart = ListCart();
            dataCart pd = listInCart.Find(p => p.ProductId == ProductId);
            listInCart.Remove(pd);
            return RedirectToAction("Cart");
        }

        [HttpGet]
        public ActionResult CustomerAddress()
        {
            //Find PhoneNumber in db CustomerAddress 
            CustomerAddress cs = db.CustomerAddresses.FirstOrDefault(p => p.CustomerPhoneNumer == Session["PhoneNumber"].ToString());
            if (cs == null)
            {
                return View();
            }
            else
            {
                return View(cs);
            }
        }
        [HttpPost]
        public ActionResult CustomerAddress(FormCollection frm, CustomerAddress cs)
        {
            var province = frm["province"];
            var district = frm["district"];
            var number = frm["number"];
            var street = frm["street"];
            var ward = frm["ward"];

            //Check null data from view
            if (String.IsNullOrEmpty(province))
            {
                ViewData["ProvinceError"] = "*This is required!";
            }
            else if (String.IsNullOrEmpty(district))
            {
                ViewData["DistrictError"] = "*This is required!";
            }
            else if (String.IsNullOrEmpty(number))
            {
                ViewData["NumberError"] = "*This is required!";
            }
            else if (String.IsNullOrEmpty(street))
            {
                ViewData["StreetError"] = "*This is required!";
            }
            else if (String.IsNullOrEmpty(ward))
            {
                ViewData["WardError"] = "*This is required!";
            }
            else
            {
                cs = db.CustomerAddresses.FirstOrDefault(p => p.CustomerPhoneNumer == Session["PhoneNumber"].ToString());
                String pn = Session["PhoneNumber"].ToString();
                if (cs != null)
                {
                    cs.AddressProvince = province.ToString();
                    cs.AddressDistrict = district.ToString();
                    cs.AddressNumber = number;
                    cs.AddressStreet = street;
                    cs.AddressWard = ward;

                    db.SubmitChanges();
                    return RedirectToAction("Order");
                }
            }

            RedirectToAction("UpdateFromCart");
            return this.CustomerAddress();
        }

        public ActionResult Order()
        {
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Cart");
            }

            List<dataCart> listInCart = ListCart();
            ViewBag.TotalAmount = TotalAmount();
            ViewBag.TotalPrice = TotalPrice();

            return View(listInCart);
        }


        private int TotalAmount()
        {
            int sum = 0;
            List<dataCart> listInCart = Session["Cart"] as List<dataCart>;
            if (listInCart != null)
            {
                sum = listInCart.Sum(p => p.Amount);
            }

            return sum;
        }
        private double TotalPrice()
        {
            double sum = 0;
            List<dataCart> listInCart = Session["Cart"] as List<dataCart>;
            if (listInCart != null)
            {
                sum = listInCart.Sum(p => p.TotalPrice);
            }

            return sum;
        }
        public ActionResult Cart()
        {
            List<dataCart> listInCart = ListCart();
            if (listInCart.Count == 0)
            {
                return RedirectToAction("Index","Home");
            }
            ViewBag.TotalAmount = TotalAmount();
            ViewBag.TotalPrice = TotalPrice();
            return View(listInCart);
        }
        public ActionResult CartPartial()
        {
            ViewBag.TotalAmount = TotalAmount();
            ViewBag.TotalPrice = TotalPrice();
            return PartialView();
        }
    }
}