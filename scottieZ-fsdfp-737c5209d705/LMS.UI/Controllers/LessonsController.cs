using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.Data.EF;
using System.IO;
using System.Drawing;

namespace LMS.UI.Controllers
{
    public class LessonsController : Controller
    {
        public LMSEntities db = new LMSEntities();

        //youtube stuff

            public void GetUrlByID(int id)
        {
            var GetUrl = db.Lessons.Where(x => x.LessonID == id)
                .Select(x => x.VideoUrl);               
            //var GetUrl = db.Lessons.Where(x => x.LessonID == 10)
            //    .Select(x => x.VideoUrl);
            //url = GetUrl.ToString();
        }


        public ActionResult YouTubeTest2()
        {
            var LessIdSelect = db.Lessons
                .Where(x => x.LessonID > 1);
            ViewBag.LessIdSelect = LessIdSelect.ToList();
            ViewBag.LessIdCount = LessIdSelect.Count();

            return View();
        }

        // GET: Lessons
        public ActionResult Index()
        {
            var lessons = db.Lessons.Include(l => l.Cours);
            return View(lessons.ToList());
        }

        // GET: Lessons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        // GET: Lessons/Create
        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName");
            return View();
        }

        public ActionResult YouTubeTest()
        {
            ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "LessonTitle");
            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LessonID,LessonTitle,CourseID,Introduction,VideoUrl,PdfFileName,IsActive")] Lesson lesson
            , HttpPostedFileBase pdfFileName)
        {

            if (ModelState.IsValid)
            {
                #region PDF Upload
                string pdfName = "";//filename
                if (pdfFileName != null && pdfFileName.ContentLength > 0 )
                {
                    //string _FileName = Path.GetFileName(pdfFileName.FileName);
                    //string _path = Path.Combine(Server.MapPath("~/Content/documents/"), _FileName);
                    //PdfFileName.SaveAs(_path);

                    pdfName = pdfFileName.FileName;
                    //string ext = pdfName.Substring(pdfName.LastIndexOf("."));
                    string pdfExt = Path.GetExtension(pdfFileName.FileName).ToLower();
                    string[] allowedExts = { ".pdf" };
                    //only pdf's
                    if (allowedExts.Contains(pdfExt))
                    {
                        pdfName = DateTime.Now.ToString("yyyyMMdd") + Path.GetFileName(pdfFileName.FileName) ;
                       //pdfName = DateTime.Now.Year + pdfExt;
                        //save with timestamp
                        // if the simple way works, add this later pdfName = DateTime.Now.ToString("yyyyMMdd" + "_" + pdfName + pdfExt);
                        //save to server
                        pdfFileName.SaveAs(Server.MapPath("~/Content/documents/" + pdfName));
                        lesson.PdfFileName = pdfName;
                    }
                }
                else
                {
                    lesson.PdfFileName = null;
                }
                //file value and pdfFileName value are the same

                #endregion

                //#region YouTubeID 
                //if (lesson.VideoUrl != null)
                //{

                //    // function youtube_parser(url){
                //    //                        var regExp = /^.* ((youtu.be\/)| (v\/)| (\/ u\/\w\/)| (embed\/)| (watch\?))\?? v ?=? ([^#\&\?]*).*/;
                //    //    var match = url.match(regExp);
                //    //                        return (match && match[7].length == 11) ? match[7] : false;


                //    var v = lesson.VideoUrl.IndexOf("v=");
                //    var amp = lesson.VideoUrl.IndexOf("&", v);
                //    string vid;
                //    // if the video id is the last value in the url
                //    if (amp == -1)
                //    {
                //        vid = lesson.VideoUrl.Substring(v + 2);
                //        // if there are other parameters after the video id in the url
                //    }
                //    else
                //    {
                //        vid = lesson.VideoUrl.Substring(v + 2, amp - (v + 2));
                //    }
                //    lesson.VideoUrl = vid;
                //    ViewBag.VideoID = vid;
                //}
                //#endregion

                db.Lessons.Add(lesson);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", lesson.CourseID);
            return View(lesson);
        }

        // GET: Lessons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", lesson.CourseID);
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LessonID,LessonTitle,CourseID,Introduction,VideoUrl,PdfFileName,IsActive")] Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", lesson.CourseID);
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lesson lesson = db.Lessons.Find(id);
            db.Lessons.Remove(lesson);
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
