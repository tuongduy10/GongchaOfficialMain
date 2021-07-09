using GongchaOfficial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GongchaOfficial.Controllers
{
    public class OrderController : Controller
    {
        dbGongchaDataContext db = new dbGongchaDataContext();
        // GET: Order
        public ActionResult Index()
        {
            return View();
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
        public ActionResult CustomerAddress(FormCollection frm,CustomerAddress cs)
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
            else if(String.IsNullOrEmpty(district))
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
                }
            }


            return this.CustomerAddress();
        }
    }
}