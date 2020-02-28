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



namespace LMS.UI.Controllers
{
    public class LessonViewsController : Controller
    {
        private LMSEntities db = new LMSEntities();

        /*This is where the logic needs to go for the Course Completion stuff
            Some thoughts on this..
            Loop though the course for lesson views tied to the user, then redirect to the create of course completion

            Create a list of records of how many lesson views in a course by the UserID, and if that number is == 3 (in this case)
                ,then create a course completion and send the email in the course completion actions (or maybe here.. IDK yet..)                        
            */
        //get : LessonViewList
        public ActionResult GetViewList()
        {
            //var Views = db.LessonViews.ToList();
            //foreach (var item in db.Employees.Select(x=> x.UserID).Where(db.LessonViews)
            //{

            //}



            return View();
        }



        // GET: LessonViews
        public ActionResult Index()
        {
            var lessonViews = db.LessonViews.Include(l => l.Lesson);
            var curUser = User.Identity.GetUserId();
            if (User.IsInRole("Admin"))
            {
                return View(lessonViews.ToList());
            }

            if (User.IsInRole("Manager"))
            {
                var empId = db.Employees.Select(b => b.ReportsToID).ToString();
                var managerLink = db.Managers.Where(x => x.UserID == curUser).Where(a => a.ManagerID.ToString() == empId);

                return View(managerLink.ToList());

            }

            if (User.IsInRole("Employee"))
            {
                var empLV = db.LessonViews.Where(x => x.UserID == curUser);
                return View(empLV.ToList());
            }



            return View(lessonViews.ToList());
        }

        // GET: LessonViews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LessonView lessonView = db.LessonViews.Find(id);
            if (lessonView == null)
            {
                return HttpNotFound();
            }
            return View(lessonView);
        }

        // GET: LessonViews/Create
        public ActionResult Create()
        {
            ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "LessonTitle");
            return View();
        }

        // POST: LessonViews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        /*This is where the logic needs to go for the Course Completion stuff
       Some thoughts on this..
       Loop though the course for lesson views tied to the user, then redirect to the create of course completion

       Create a list of records of how many lesson views in a course by the UserID, and if that number is == 3 (in this case)
           ,then create a course completion and send the email in the course completion actions (or maybe here.. IDK yet..)                        
       */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LessonViewID,UserID,LessonID,DateViewed")] LessonView lessonView)
        {
            lessonView.UserID = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {   //the current lessonviews courseID
                var currentCourseID = (from l in db.Lessons
                                       join c in db.Courses on l.CourseID equals c.CourseID
                                       where l.LessonID == lessonView.LessonID
                                       select c.CourseID).FirstOrDefault();
                //list of lession views for the current user
                var curUser = User.Identity.GetUserId();
                var userLVs = (from LV in db.LessonViews
                               where LV.UserID == curUser
                               select LV).ToList();   
                
                db.LessonViews.Add(lessonView);
                db.SaveChanges();                                                   
                //count the number of lessonviews with this current courseID
                var countLVsInCourse = userLVs.Where(x => x.Lesson.CourseID == currentCourseID).Count();            
                if (countLVsInCourse >= 2)
                {
                    //this is where we create the CourseCompletion
                    //based on that courseID and the current userID
                    CourseCompletion courseCompletion = new CourseCompletion();
                    courseCompletion.UserID = curUser;
                    courseCompletion.CourseID = currentCourseID;
                    courseCompletion.DateCompleted = DateTime.Now;
                    db.CourseCompletions.Add(courseCompletion);
                    db.SaveChanges();
                    //send an email to that persons manager..
                    var curUserNameFirst = db.Employees.Where(x => x.UserID == curUser).Select(x => x.FirstName).ToString();
                    var curUserNameLast = db.Employees.Where(x => x.UserID == curUser).Select(x => x.LastName).ToString();
                    var curEmp = db.Employees.Where(x => x.UserID == curUser).FirstOrDefault();

                    string body = string.Format($"Name:{curEmp.FullName} , <br />Has Completed Course {currentCourseID}<br /> Completion Time: {courseCompletion.DateCompleted} <br />");
                    MailMessage msg = new MailMessage("Admin@scottiez.com", 
                       "Admin@scottiez.com",
                       "Course Completion Alert",
                       body);
                    
                    msg.IsBodyHtml = true; //body of the message is HTML
                                           //msg.cc.Add("ziggish@att.net");  sends a carbon copy
                                           //msg.Bcc.Add("ziggish@att.net"); //send a blind carbon copy so that no one knows that you got a CC
                    msg.Priority = MailPriority.High; 
                    //create and configure the SMTP client  ... Standard Mail Transfer Protocol (mail carrier)
                    SmtpClient client = new SmtpClient("mail.scottiez.com");  //mail person
                    client.Credentials = new NetworkCredential("Admin@scottiez.com", "P@ssw0rd"); 
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
                    }
                    return RedirectToAction("Index");
                }

                ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "LessonTitle", lessonView.LessonID);
                return View("Index");
            }
            return View();
        }
// GET: LessonViews/Edit/5
public ActionResult Edit(int? id)
{
    if (id == null)
    {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    }
    LessonView lessonView = db.LessonViews.Find(id);
    if (lessonView == null)
    {
        return HttpNotFound();
    }
    ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "LessonTitle", lessonView.LessonID);
    return View(lessonView);
}

// POST: LessonViews/Edit/5
// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Edit([Bind(Include = "LessonViewID,UserID,LessonID,DateViewed")] LessonView lessonView)
{
    if (ModelState.IsValid)
    {
        db.Entry(lessonView).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
    }
    ViewBag.LessonID = new SelectList(db.Lessons, "LessonID", "LessonTitle", lessonView.LessonID);
    return View(lessonView);
}

// GET: LessonViews/Delete/5
public ActionResult Delete(int? id)
{
    if (id == null)
    {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    }
    LessonView lessonView = db.LessonViews.Find(id);
    if (lessonView == null)
    {
        return HttpNotFound();
    }
    return View(lessonView);
}

// POST: LessonViews/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public ActionResult DeleteConfirmed(int id)
{
    LessonView lessonView = db.LessonViews.Find(id);
    db.LessonViews.Remove(lessonView);
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
