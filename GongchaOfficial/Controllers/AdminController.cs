using GongchaOfficial.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GongchaOfficial.Controllers
{
    public class AdminController : Controller
    {
        dbGongchaDataContext db = new dbGongchaDataContext();  

        // GET: Admin
        //Trang chủ
        public ActionResult Index()
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        //Login to admin page
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["adminAccount"];
            var matkhau = collection["adminPassword"];

            if(String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if(String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }    
            else
            {
                Admin ad = db.Admins.SingleOrDefault(n => n.AdminAccount == tendn && n.AdminPassword == matkhau);
                if (ad != null)
                {
                    Session["AdminAccount"] = tendn;
                    return RedirectToAction("Index","Admin");
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
                    
            }    
            return View();
        }

        //Log Off
        public ActionResult LogOff()
        {
            Session.Clear();
            return RedirectToAction("Login","Admin");
        }


        public ActionResult Product(int? page)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }            
            else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 6;


                return View(db.Products.ToList().OrderBy(n => n.ProductId).ToPagedList(pageNumber, pageSize));
            }
        }

        /*
        *Products
        */

        //Create new Product
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }                
            else
            {
                Product product = new Product();
                ViewBag.ProductId = product.ProductId;
                ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
                ViewBag.SexId = new SelectList(db.Sexes, "SexId", "Sex1", product.SexId);


                return View();
            }
        }
        [HttpPost]
        public ActionResult Create(Product product, HttpPostedFileBase fileUpload,FormCollection frm)
        {                  
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Luu ten fie, luu y bo sung thu vien using System.IO;
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/Content/style/img"), fileName);
                    //Kiem tra hình anh ton tai chua?
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    product.ProductImage = fileName;
                    product.ProductStatus = frm["Status"].ToString();
                    //Luu vao CSDL
                    db.Products.InsertOnSubmit(product);
                    db.SubmitChanges();
                }
                return RedirectToAction("Product");
            }
        }
        //Detail Product
        public ActionResult Details(string id)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }               
            else
            {
                Product product = db.Products.SingleOrDefault(n => n.ProductId == id);
                ViewBag.ProductId = product.ProductId;
                if (product == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(product);
            }
        }
        
        
        //Delete Products
        [HttpGet]
        public ActionResult Delete(string id)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }              
            else
            {
                //Lay ra doi tuong can xoa theo ma
                Product product = db.Products.SingleOrDefault(n => n.ProductId == id);
                ViewBag.ProductId = product.ProductId;
                if (product == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(product);
            }
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Xacnhanxoa(string id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            Product product = db.Products.SingleOrDefault(n => n.ProductId == id);
            ViewBag.ProductId = product.ProductId;
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.Products.DeleteOnSubmit(product);            
            db.SubmitChanges();
            return RedirectToAction("Product");
        }
        
        
        // Edit Products
        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }              
            else
            {
                //Lay ra doi tuong theo ma
                Product product = db.Products.SingleOrDefault(n => n.ProductId == id);

                ViewBag.ProductId = product.ProductId;
                ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
                ViewBag.SexId = new SelectList(db.Sexes, "SexId", "Sex1", product.SexId);
                List<Size> listSize = db.Sizes.ToList();
                ViewBag.Size = db.Sizes.ToList();


                if (product == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(product);
            }
        }
        [HttpPost]
        public ActionResult Edit(string id,FormCollection frm,HttpPostedFileBase fileUpload)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login","Admin");
            }
            else
            {
                Product product = db.Products.SingleOrDefault(n => n.ProductId == id);
                ViewBag.ProductId = product.ProductId;

                if (ModelState.IsValid)
                {
                    if (fileUpload != null)
                    {
                        var fileName = Path.GetFileName(fileUpload.FileName);
                        var filePath = Path.Combine(Server.MapPath("~/Content/style/img"), fileName);

                        if (System.IO.File.Exists(filePath))
                        {
                            ViewBag.ThongBao = "Hình đã tồn tại";
                        }
                        else
                        {
                            fileUpload.SaveAs(filePath);
                            product.ProductImage = fileName;
                        }
                    }
                    product.ProductReleaseDate = DateTime.Parse(frm["txtReleaseDate"]);
                    product.ProductStatus = frm["Status"].ToString();
                    UpdateModel(product);
                    db.SubmitChanges();
                }

                return RedirectToAction("Product");
            }
                                    
        }

        /*
         *Category
         */
        
        public ActionResult CategoryIndex()
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login","Admin");
            }
            else
            {
                var listCategory = db.Categories.ToList();
                return View(listCategory);
            }
            
        }

        //Detail
        public ActionResult DetailCategory(string id)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                Category ctg = db.Categories.SingleOrDefault(p => p.CategoryId == id);
                ViewBag.CategorySize = db.Sizes.Where(p => p.SizeId == id).ToList();
                return View(ctg);
            }
        }
        
        
        //Delete
        [HttpGet]
        public ActionResult DeleteCategory(string id)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                Category ct = db.Categories.SingleOrDefault(p => p.CategoryId == id);
                return View(ct);
            }           
        }
        [HttpPost, ActionName("DeleteCategory")]
        public ActionResult DeleteSubmitCategory(string id)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                Category ct = db.Categories.SingleOrDefault(p => p.CategoryId == id);
                List<Size> s = db.Sizes.Where(p => p.SizeId == id).ToList();

                db.Categories.DeleteOnSubmit(ct);
                db.Sizes.DeleteAllOnSubmit(s);
                db.SubmitChanges();
                return RedirectToAction("CategoryIndex");
            }
        }
        
        
        //Create 
        [HttpGet]
        public ActionResult CreateCategory(string id)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                Category ct = new Category();
                ViewBag.CategoryId = ct.CategoryId;
                ViewBag.SexId = new SelectList(db.Sexes, "SexId", "Sex1", ct.SexId);
                return View();
            }
        }
        [HttpPost]
        public ActionResult CreateCategory(Category ct,FormCollection frm)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    ct.CategoryStatus = frm["CategoryStatus"].ToString();
                    db.Categories.InsertOnSubmit(ct);
                    db.SubmitChanges();
                }

                return RedirectToAction("CategoryIndex");
            }
        }

        
        //Edit
        [HttpGet]
        public ActionResult EditCategory(string id)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                Category ct = db.Categories.SingleOrDefault(p => p.CategoryId == id);
                ViewBag.SexId = new SelectList(db.Sexes, "SexId", "Sex1", ct.SexId);

                return View(ct);
            }  
        }
        [HttpPost]
        public ActionResult EditCategory(string id, FormCollection frm)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                Category ct = db.Categories.SingleOrDefault(p => p.CategoryId == id);
                ct.CategoryStatus = frm["CategoryStatus"].ToString();
                UpdateModel(ct);
                db.SubmitChanges();
                return RedirectToAction("CategoryIndex");
            }
        }


        /*
         *Size
         */

        //Add size
        [HttpGet]
        public ActionResult AddSize(string id)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                ViewBag.Size = db.Sizes.Where(p => p.SizeId == id).ToList();
                
                return View();
            }          
        }

        [HttpPost]
        public ActionResult AddSize(string id,FormCollection frm)
        {
            if (Session["AdminAccount"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            else
            {               
                if (ModelState.IsValid)
                {
                    ViewBag.Size = db.Sizes.Where(p => p.SizeId == id).ToList();
                    Size newSize = new Size();
                    newSize.SizeId = id;
                    newSize.SizeValue = frm["txtSizeValue"];

                    db.Sizes.InsertOnSubmit(newSize);
                    db.SubmitChanges();
                }
                
                return View(id);
            }            
        }
    }
}