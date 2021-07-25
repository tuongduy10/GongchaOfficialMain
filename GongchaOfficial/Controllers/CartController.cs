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

        //Cart
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
        //Create Cart:
        public ActionResult Cart()
        {
            List<dataCart> listInCart = ListCart();
            List<Size> listSize = db.Sizes.ToList();

            if (listInCart.Count == 0)
            {
                return RedirectToAction("Index","Home");
            }
            ViewBag.TotalAmount = TotalAmount();
            ViewBag.TotalPrice = TotalPrice();
            ViewBag.Size = listSize;

            return View(listInCart);
        }
        //Cart Icon for _LayoutUser
        public ActionResult CartPartial()
        {
            ViewBag.TotalAmount = TotalAmount();
            ViewBag.TotalPrice = TotalPrice();
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddToCart(string ProductId,string size, string url)
        {
            //Tạo listCast và lấy sản phẩm từ Cart
            List<dataCart> listInCart = ListCart();
            dataCart pd = listInCart.Find(p => p.ProductId == ProductId && p.ProductSize == size);           

            if (pd == null)
            {
                pd = new dataCart(ProductId);//Tạo mới 1 sản phẩm với ProductId được truyền vào
                pd.ProductSize = size;
                listInCart.Add(pd);//Thêm sản phẩm vào giỏ hàng
            }
            else//(pd !=null)
            {
                pd.Amount++;
            }
                            
            return Json("OK");
        }

        [HttpPost]
        public ActionResult UpdateAmount(int ProductPosition, int Amount, string url)
        {
            List<dataCart> listCart = ListCart();
            dataCart ps = listCart.ElementAt(ProductPosition);
            if (ps != null)
            {
                ps.Amount = Amount;
            }
            ViewBag.TotalAmount = TotalAmount();
            ViewBag.TotalPrice = TotalPrice();
            return Json("OK");
        }
        [HttpPost]
        public ActionResult UpdateSize(int ProductPosition,string Size, string url)
        {
            List<dataCart> listCart = ListCart();
            dataCart ps = listCart.ElementAt(ProductPosition);
            if (ps != null)
            {
                ps.ProductSize = Size;
            }

            return Json("OK");
        }

        public ActionResult RemoveFromCart(int ProductPosition, string url)
        {
            List<dataCart> listInCart = ListCart();
            dataCart ps = listInCart.ElementAt(ProductPosition);
            listInCart.Remove(ps);
            return RedirectToAction("Cart");
        }

        //Address
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

            return this.CustomerAddress();
        }

        //Order
        public ActionResult Order()
        {
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Cart");
            }

            List<dataCart> listInCart = ListCart();

            ViewBag.TotalAmount = TotalAmount();
            ViewBag.TotalPrice = TotalPrice();
            ViewBag.Address = Address();
            
            return View(listInCart);
        }
        //Save Order
        public ActionResult SaveOrder()
        {
            List<dataCart> listInCart = ListCart();

            //Add Bill
            Bill bill = new Bill();
            
            bill.BillTimeSet = DateTime.Now;
            bill.BillPrice = TotalPrice();
            bill.CustomerPhoneNumber = Session["PhoneNumber"].ToString();

            db.Bills.InsertOnSubmit(bill);
            db.SubmitChanges();

            
            foreach(var item in listInCart)
            {
                //Add Bill Detail
                BillDeTail billdetail = new BillDeTail();

                billdetail.BillId = bill.BillId;
                billdetail.NumericalOrder = listInCart.IndexOf(item)+1;
                billdetail.ProductId = item.ProductId;
                billdetail.Price = item.ProductPrice;
                billdetail.Amount = item.Amount;
                billdetail.PriceTotal = item.TotalPrice;
                billdetail.Size = item.ProductSize;

                db.BillDeTails.InsertOnSubmit(billdetail);
            }

            db.SubmitChanges();
            Session["Cart"] = null;


            return RedirectToAction("Index","Women");
        }


        //Data: Price, Amount, Size
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
        private String Address()
        {
            CustomerAddress ca;
            ca = db.CustomerAddresses.FirstOrDefault(p => p.CustomerPhoneNumer == Session["PhoneNumber"].ToString());

            return ca.AddressNumber + " " + ca.AddressStreet + ", " + ca.AddressWard + ", " + ca.AddressDistrict + ", " + ca.AddressProvince;
        }        

    }
}