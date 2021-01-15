using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Projekt_wycieczki_koncowy.Models;

namespace Projekt_wycieczki_koncowy.Controllers
{
    public class AccountController : Controller
    {
        private projekt_baza_danych_wycieczki db = new projekt_baza_danych_wycieczki();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.User.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idUser,UserName,Password,Rola,ConfirmPassword")] User user)
        {
            if (db.User.Where(u => u.UserName == user.UserName).Any())
            {
                ModelState.AddModelError("UserName", "Nazwa użytkownika zajęta");
                return View(user);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    user.ConfirmPassword = user.Password;
                    db.User.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(user);
            }
        }

 
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idUser,UserName,Password,Rola,ConfirmPassword")] User user)
        {
            if (ModelState.IsValid)
            {
                user.ConfirmPassword = user.Password;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            if (user.UserName == User.Identity.Name)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(user);
            }
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.User.Find(id);
            db.User.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Models.Membership model)
        {
            using (var context = new projekt_baza_danych_wycieczki())
            {
                bool isValid = context.User.Any(x => x.UserName == model.UserName && x.Password == model.Password);
                if (isValid)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Błędna nazwa lub hasło.");
                return View();
            }
        }
        //
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(User model)
        {
            if (db.User.Where(u => u.UserName == model.UserName).Any())
            {
                ModelState.AddModelError("UserName", "Nazwa użytkownika zajęta");
                return View(model);
            }
            else
            {
                using (var context = new projekt_baza_danych_wycieczki())
                {
                    model.Rola = "User";
                    context.User.Add(model);
                    context.SaveChanges();
                }
                return RedirectToAction("login");
            }
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
