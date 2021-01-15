using Projekt_wycieczki_koncowy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Projekt_wycieczki_koncowy.Controllers
{
    public class WycieczkisController : Controller
    {
        private projekt_baza_danych_wycieczki db = new projekt_baza_danych_wycieczki();

        public ActionResult Index(string sortOrder, DateTime? fromDate, DateTime? toDate)
        {
            var model = new Models_together();
            model.WycieczkiTogEnum = db.Wycieczki.ToList();
            model.ImagesTogEnum = db.Images.ToList();
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            var wyc_search = from s in model.WycieczkiTogEnum select s;

            if(fromDate.HasValue)
            {
                model.WycieczkiTogEnum = model.WycieczkiTogEnum.Where(s => s.Data >= fromDate);
            }
            if (toDate.HasValue)
            {
                model.WycieczkiTogEnum = model.WycieczkiTogEnum.Where(s => s.Data <= toDate);
            }
            if (fromDate.HasValue && toDate.HasValue)
            {
                model.WycieczkiTogEnum = model.WycieczkiTogEnum.Where(s => s.Data >= fromDate && s.Data <= toDate);
            }

            return View(model);
        }

        public ActionResult DeleteImage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            foreach (var img in db.Images.Where(x => x.WycieczkaID == id))
            {
                db.Images.Remove(img);
            }
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult DeleteVideo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            db.Wycieczki.Find(id).VideoFile = null;

            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = new Models_together();
            model.WycieczkiTog = db.Wycieczki.Find(id);
            model.WycieczkiTogEnum = db.Wycieczki.ToList();
            model.ImagesTogEnum = db.Images.ToList();

            if (model.WycieczkiTog == null)
            {
                return HttpNotFound();
            }
            return PartialView(model);
        }

        [Authorize(Roles = "User,Admin")]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Msc_start,Msc_end,Data,Liczba_km,Liczba_dni,VideoFile")] Wycieczki wycieczki, [Bind(Include = "Id,ImgData,ProductID")] Images images, List<HttpPostedFileBase> ImgData, HttpPostedFileBase videoFile)
        {
            if (ModelState.IsValid)
            {
                if (videoFile != null)
                {
                    if (videoFile.ContentLength < 104857600)
                    {
                        string videoname = Path.GetFileName(videoFile.FileName);
                        var un = wycieczki.Id + "_unic_" + videoname;
                        string filepath = Path.Combine(Server.MapPath("~/UploadedVideos/"), un);
                        videoFile.SaveAs(filepath);
                        wycieczki.VideoFile = "/UploadedVideos/" + un;
                    }
                }
                else
                {

                    wycieczki.VideoFile = null;
                }
                db.Wycieczki.Add(wycieczki);

                if ( ImgData[0] != null)
                {
                    foreach (var item in ImgData)
                    {
                        if (item.ContentLength > 0)
                        {
                            var image = Path.GetFileName(item.FileName);
                            var un = wycieczki.Id + "_unic_" + image;
                            var path = Path.Combine(Server.MapPath("~/UploadedFiles/"), un);
                            item.SaveAs(path);
                            images.ImgData = "/UploadedFiles/" + un;
                            images.WycieczkaID = wycieczki.Id;
                            db.Images.Add(images);
                            db.SaveChanges();
                        }
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(wycieczki);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wycieczki wycieczki = db.Wycieczki.Where(s => s.Id == id).FirstOrDefault();
            if (wycieczki == null)
            {
                return HttpNotFound();
            }
            return View(wycieczki);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Msc_start,Msc_end,Data,Liczba_km,Liczba_dni,VideoFile")] Wycieczki wycieczki, [Bind(Include = "Id,ImgData,ProductID")] Images images, List<HttpPostedFileBase> ImgData, HttpPostedFileBase video)
        {
            if (ModelState.IsValid)
            {
                string actual_video = wycieczki.VideoFile;
                if ( video !=null)
                {
                    if (video.ContentLength < 104857600)
                    {
                        string videoname = Path.GetFileName(video.FileName);
                        var un = wycieczki.Id + "_unic_" + videoname;
                        string filepath = Path.Combine(Server.MapPath("~/UploadedVideos/"), un);
                        video.SaveAs(filepath);
                        wycieczki.VideoFile = "/UploadedVideos/" + un;
                        db.Entry(wycieczki).State = EntityState.Modified;
                    }
                }


                if (video ==null)
                {
                    if(wycieczki.VideoFile==null)
                    {
                            db.Entry(wycieczki).State = EntityState.Modified;
                    }
                    if(wycieczki.VideoFile!=null)
                    {
                        wycieczki.VideoFile = actual_video;
                        db.Entry(wycieczki).State = EntityState.Modified;
                    }
                }
                db.SaveChanges();


                if (ImgData[0] != null)
                {
                    foreach (var img in db.Images.Where(x => x.WycieczkaID == wycieczki.Id))
                    {
                        db.Images.Remove(img);

                    }
                    db.Entry(wycieczki).State = EntityState.Modified;
                    db.SaveChanges();
                    foreach (var item in ImgData)
                    {
                        if (item.ContentLength > 0)
                        {
                            var image = Path.GetFileName(item.FileName);
                            var un = wycieczki.Id + "_unic_" + image;
                            var path = Path.Combine(Server.MapPath("~/UploadedFiles/"), un);
                            item.SaveAs(path);
                            images.ImgData = "/UploadedFiles/" + un;
                            images.WycieczkaID = wycieczki.Id;
                            db.Images.Add(images);
                            db.SaveChanges();
                        }
                    }

                    
                    
                }
                db.Entry(wycieczki).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(wycieczki);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = new Models_together();
            model.WycieczkiTog = db.Wycieczki.Find(id);
            model.WycieczkiTogEnum = db.Wycieczki.ToList();
            model.ImagesTogEnum = db.Images.ToList();

            if (model.WycieczkiTog == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Wycieczki wycieczki = db.Wycieczki.Find(id);
            foreach (var img in db.Images.Where(x => x.WycieczkaID == id))
            {
                db.Images.Remove(img);
            }
            db.Wycieczki.Remove(wycieczki);
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
    }
}
