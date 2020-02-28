using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.Data.EF;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.Collections;


namespace LMS.UI.Controllers
{
    public class CourseCompletionsController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: CourseCompletions
        public ActionResult Index()
        {

            var courseCompletions = db.CourseCompletions.Include(c => c.Cours);

            var curUser = User.Identity.GetUserId();
            if (User.IsInRole("Admin"))
            {
                return View(courseCompletions.ToList());
            }

            if (User.IsInRole("Manager"))
            {
                var empId = db.Employees.Select(b => b.ReportsToID).ToString();
                var managerLink = db.Managers.Where(x => x.UserID == curUser).Where(a => a.ManagerID.ToString() == empId);

                return View(managerLink.ToList());

            }

            if (User.IsInRole("Employee"))
            {
                var empLV = db.CourseCompletions.Where(x => x.UserID == curUser);
                return View(empLV.ToList());
            }           
            return View(courseCompletions.ToList());
        }

        // GET: CourseCompletions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCompletion courseCompletion = db.CourseCompletions.Find(id);
            if (courseCompletion == null)
            {
                return HttpNotFound();
            }
            return View(courseCompletion);
        }

        // GET: CourseCompletions/Create
        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName");
            return View();
        }

        // POST: CourseCompletions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseCompletionID,UserID,CourseID,DateCompleted")] CourseCompletion courseCompletion)
        {
            //var CurUser = User.Identity.GetUserId();
            //var lessonsAmount = db.Lessons.Select(x => x.LessonID).Count();
            //var lessonViewsAmount = db.LessonViews.Select(x => x.LessonID).Count();
            //var lessonVerif = (lessonViewsAmount) - (lessonsAmount);
            if (ModelState.IsValid)
            {
                db.CourseCompletions.Add(courseCompletion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }//end modelState.isValid

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", courseCompletion.CourseID);
            return View(courseCompletion);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ContactManagerCompletion()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ContactManagerCompletion(CourseCompletion x)
        {
            //create a body for the email (these are words..)

            string body = string.Format($"Name: {x.UserID}, {x.Employee}<br />Has Completed Course {x.CourseID}<br /> Completion Time: {x.DateCompleted} <br />");

            //create and configure the mail message (this is the letter)
            MailMessage msg = new MailMessage("Admin@scottiez.com", //where we are sending from
               "Admin@scottiez.com",//where we are sending to
               x.UserID + " Course Completion Alert", //subject of the message
               body);

            //configure the mail message object (envelope)
            msg.IsBodyHtml = true; //body of the message is HTML
                                   //msg.cc.Add("ziggish@att.net");  sends a carbon copy
                                   //msg.Bcc.Add("ziggish@att.net"); //send a blind carbon copy so that no one knows that you got a CC
            msg.Priority = MailPriority.High; //we want the email to end up in their mailbox, not some other folder or something


            //create and configure the SMTP client  ... Standard Mail Transfer Protocol (mail carrier)
            SmtpClient client = new SmtpClient("mail.scottiez.com");  //mail person
            client.Credentials = new NetworkCredential("Admin@scottiez.com", "P@ssw0rd"); //like a stamp
                                                                                          //this part just attaches the username/password to the client, so we can send the email        
            client.Port = 8889;
            //the original is 25, but in case that port is being blocked, you can change the port number here
            using (client)
            {
                try
                {
                    client.Send(msg); //sending the MailMessage object
                }
                catch
                {
                    ViewBag.ErrorMessage = "There was an error sending your message, please email admin@scottiez.com";
                    return View();
                }
            }//using            
            return View();
        }



        // GET: CourseCompletions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCompletion courseCompletion = db.CourseCompletions.Find(id);
            if (courseCompletion == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", courseCompletion.CourseID);
            return View(courseCompletion);
        }

        // POST: CourseCompletions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CourseCompletionID,UserID,CourseID,DateCompleted")] CourseCompletion courseCompletion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseCompletion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", courseCompletion.CourseID);
            return View(courseCompletion);
        }

        // GET: CourseCompletions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCompletion courseCompletion = db.CourseCompletions.Find(id);
            if (courseCompletion == null)
            {
                return HttpNotFound();
            }
            return View(courseCompletion);
        }

        // POST: CourseCompletions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseCompletion courseCompletion = db.CourseCompletions.Find(id);
            db.CourseCompletions.Remove(courseCompletion);
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
