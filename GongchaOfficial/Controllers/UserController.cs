﻿using GongchaOfficial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        public ActionResult Register(FormCollection frm, Customer cs)
        {
            var name = frm["fullname"];
            var dateofbirth = String.Format("{0:MM/dd/yyy}", frm["dateofbirth"]);
            var sex = frm["sex"];
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
                //cs.CustomerSex = sex;
                cs.CustomerMail = mail;
                cs.CustomerPhoneNumer = phonenumber;
                cs.CustomerPassword = password;

                db.Customers.InsertOnSubmit(cs);
                db.SubmitChanges();

                return RedirectToAction("Login");
            }


            return this.Register();
        }
    }
}