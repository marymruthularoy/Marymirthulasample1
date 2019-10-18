using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marymirthulasample1.Models;
using System.Web.Security;
using System.Net;

namespace Marymirthulasample1.Controllers
{
    public class HomeController : Controller
    {
        marysample1Entities msample1 = new marysample1Entities();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Register_table r)
        {
            if (ModelState.IsValid)
            {
                var usr = (from u in msample1.Register_table where u.username == r.username select u).FirstOrDefault();
                if(usr==null)
                {
                    msample1.Register_table.Add(new Register_table
                    {
                        name=r.name,
                        email=r.email,
                        phone=r.phone,
                        username=r.username,
                        password=r.password,
                        confirmpassword=r.confirmpassword,
                        address=r.address,
                        datex = DateTime.Now.ToString()
                    });
                    msample1.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["AE"] = "This user name is already exist";
                    return RedirectToAction("Admin");
                }

            }

                return View(r);
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            string userName = form["userName"].ToString();
            string password = form["password"].ToString();
            var usr = (from u in msample1.Register_table where u.username == userName select u).FirstOrDefault();
            if (usr == null)
            {
                TempData["Message"] = "username or password is wrong";
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var model = msample1.Register_table.Where(x => x.username == userName && x.password == password).SingleOrDefault();
                if (model != null)
                {
                    Session["userid"] = usr.id.ToString();
                    Session["username"] = usr.username.ToString();
                    FormsAuthentication.SetAuthCookie(usr.username, false);
                    return RedirectToAction("Homepage", "Home");
                }
                else
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Login", "Home");
                }
            }
           
         
        }
        public ActionResult Homepage()
        {
            if (Session["username"] == null || Session["userid"].ToString() == null)
            {
                TempData["Message"] = "You Have To Login !!!!";
                return this.RedirectToAction("Login", "Home");
            }
            var userName = Session["username"].ToString();
            var usr = (from u in msample1.Register_table where u.username == userName select u.name).FirstOrDefault();
            ViewData["Name"] = usr;
          
            var Homelist = (from sub in msample1.Register_table orderby sub.id descending select sub).ToList();
            return View(Homelist);
        }


        public ActionResult Edit(int? Id)
        {
            if (Session["username"] == null || Session["userid"].ToString() == null)
            {
                TempData["Message"] = "You Have To Login !!!!";
                return this.RedirectToAction("Login", "Home");
            }
            //if (Id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            Register_table rt = msample1.Register_table.Find(Id);

            if (rt == null)
            {
                return HttpNotFound();
            }
            return View(rt);
        }
        [HttpPost]
        public ActionResult Edit(Register_table r)
        {
            if (Session["username"] == null || Session["userid"].ToString() == null)
            {
                TempData["Message"] = "You Have To Login !!!!";
                return this.RedirectToAction("Login", "Home");
            }
            if (ModelState.IsValid)
            {
                Register_table existing = msample1.Register_table.Find(r.id);
                
                existing.name = r.name;
                existing.email = r.email;
                existing.phone = r.phone;
                existing.password = r.password;
                existing.confirmpassword = r.confirmpassword;
                existing.address = r.address;
                if(existing.username !=r.username)
                {
                    var userCount = (from u in msample1.Register_table where u.username == r.username select u).Count();
                    if (userCount == 0)
                    {
                        existing.username = r.username;
                    }
                    else
                    {
                        TempData["AE"] = "This user name is already exist";
                        return RedirectToAction("Admin");
                    }

                }
                if (existing.datex == null)
                {
                    existing.datex = DateTime.Now.ToString();
                }
                else
                {
                    existing.datex = existing.datex;
                }
                msample1.SaveChanges();
                return RedirectToAction("Homepage", "Home");
            }

            return RedirectToAction("Homepage", "Home");
        }


        public ActionResult Details(int Id)
        {
            if (Session["username"] == null || Session["userid"].ToString() == null)
            {
                TempData["Message"] = "You Have To Login !!!!";
                return this.RedirectToAction("Login", "Home");
            }

            Register_table rt = msample1.Register_table.Find(Id);

            if (rt == null)
            {
                return HttpNotFound();
            }
            return View(rt);
        }


     
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}