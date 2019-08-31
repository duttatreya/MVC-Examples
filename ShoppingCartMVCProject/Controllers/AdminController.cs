using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingCartMVCProject.Models;

namespace ShoppingCartMVCProject.Controllers
{
    public class AdminController : Controller
    {
        MVCDBEntities db = new MVCDBEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddProduct()
        {

            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(ProductInfo pi)
        {
            if (ModelState.IsValid)
            {
                db.ProductInfoes.Add(pi);
                var res = db.SaveChanges();
                if (res > 0)
                {
                    ModelState.AddModelError("", "Product Inserted");
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult SelectUpdateProduct()
        {
            //bind the data from the database to the dropdownlist
            Session["prod"] = new SelectList(db.ProductInfoes.ToList(),"pid","pname");
            return View();
        }

        
        [HttpPost]
        public ActionResult SelectUpdateProduct(string Command)
        {   
            if(Command == "Select By Id")
            {
                int id = Convert.ToInt32(Request.Form["ddlprod"]);
                var data = db.ProductInfoes.Where(x => x.pid == id).SingleOrDefault();
                Session["data"] = data;
                return View(Session["data"]);
            }
            if(Command == "Update")
            {
                int id = Convert.ToInt32(Request.Form["txtpid"]);
                string newName = Request.Form["txtpname"].ToString();
                decimal newPrice = Convert.ToDecimal(Request.Form["txtprice"]);
                var old = db.ProductInfoes.Where(x => x.pid == id).SingleOrDefault();
                old.price = newPrice;
                old.pname = newName;
                var res = db.SaveChanges();
                if(res > 0)
                {
                    ModelState.AddModelError("", "Data Updated");
                    
                }
                return View();

            }

            return View();
        }
        [HttpGet]
        public ActionResult EditProducts()
        {
            var data = db.ProductInfoes;
            return View(data.ToList());
        }
        

        public ActionResult UpdateProduct(int id)
        {
            var data = db.ProductInfoes.Where(x => x.pid == id).SingleOrDefault();
            Session["oldData"] = data;
            return View(Session["oldData"]);
        }
        [HttpPost]
        public ActionResult UpdateProduct()
        {
            int id = Convert.ToInt32(Request.Form["txtpid"]);
            string newpname = Request.Form["txtpname"].ToString();
            decimal newprice = Convert.ToDecimal(Request.Form["txtprice"]);
            var old = db.ProductInfoes.Where(x => x.pid == id).SingleOrDefault();
            old.pname = newpname;
            old.price = newprice;
            var res = db.SaveChanges();
            if(res > 0)
            {
                ModelState.AddModelError("", "Data Updated");
            }
            return View();
        }

        [HttpGet]
        public ActionResult DeleteProduct()
        {
            Session["delete"] = new SelectList(db.ProductInfoes.ToList(), "pid", "pname");
            return View();
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id=0)
        {
            id = Convert.ToInt32(Request.Form["ddlprod"]);
            var pi = db.ProductInfoes.Where(x => x.pid == id).SingleOrDefault();
            db.ProductInfoes.Remove(pi);
            var res = db.SaveChanges();
            if(res > 0)
            {
                ModelState.AddModelError("", "Deleted Product Successfully");
            }
            return View();

        }

        public ActionResult ViewCustomer()
        {
            var data = db.PersonInfoes.Where(x => x.type == "Customer").ToList();
            return View(data);

        }
    }
}