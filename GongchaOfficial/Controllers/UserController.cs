using BookStoreEntity.Models;
using GongchaOfficial.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace GongchaOfficial.Controllers
{
    public class UserController : Controller
    {
        dbGongchaDataContext db = new dbGongchaDataContext();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(FormCollection frm, Customer cs,CustomerAddress ca)
        {
            var name = frm["fullname"];
            var dateofbirth = String.Format("{0:MM/dd/yyy}", frm["dateofbirth"]);
            var sex = frm["Gender"].ToString();
            var mail = frm["mail"];

            var phonenumber = frm["phonenumber"];
            var password = frm["password"];


            if (String.IsNullOrEmpty(name))
            {
                ViewData["ErrorName"] = "Name can't be empty!";
            }
            else if (String.IsNullOrEmpty(dateofbirth))
            {
                ViewData["ErrorDate"] = "Date can't be empty!";
            }
            else if (String.IsNullOrEmpty(sex))
            {
                ViewData["ErrorGender"] = "Please check your gender!";
            }
            else if (String.IsNullOrEmpty(mail))
            {
                ViewData["ErrorMail"] = "Mail can't be empty!";
            }
            else if (String.IsNullOrEmpty(phonenumber))
            {
                ViewData["ErrorPhone"] = "Phone number can't be empty!";
            }
            else if (String.IsNullOrEmpty(password))
            {
                ViewData["ErrorPassword"] = "Please type your password!";
            }
            else
            {
                cs.CustomerName = name;
                cs.CustomerDateOfBirth = DateTime.Parse(dateofbirth);
                cs.CustomerSex = sex;
                cs.CustomerMail = mail;
                cs.CustomerPhoneNumer = phonenumber;
                ca.CustomerPhoneNumer = phonenumber;
                cs.CustomerPassword = password;

                db.Customers.InsertOnSubmit(cs);
                db.CustomerAddresses.InsertOnSubmit(ca);
                db.SubmitChanges();

                return RedirectToAction("SignIn");
            }


            return this.Register();
        }


        [HttpGet]
        public ActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignIn(FormCollection frm)
        {
            var phonenumber = frm["phonenumber"];
            var password = frm["password"];
            if (String.IsNullOrEmpty(phonenumber))
            {
                ViewData["ErrorPhonenumber"] = "Phone number can't be empty!";
            }
            else if (String.IsNullOrEmpty(password))
            {
                ViewData["ErrorPassword"] = "Password can't be empty!";
            }
            else
            {
                Customer cs = db.Customers.SingleOrDefault(p => p.CustomerPhoneNumer == phonenumber && p.CustomerPassword == password);
                if (cs != null)
                {                    
                    Session["CustomerName"] = cs.CustomerName.ToString();
                    Session["PhoneNumber"] = cs.CustomerPhoneNumer.ToString();
                    return RedirectToAction("Index","Home");
                }
            }

            return View();
        }
        
        public ActionResult SignOut()
        {
            Session.Clear();
            return RedirectToAction("Index","Home");
        }




      
    }
}