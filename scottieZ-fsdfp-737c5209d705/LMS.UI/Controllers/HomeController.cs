using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using LMS.Data.EF;
using LMS.UI.Models;

namespace IdentitySample.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult About()
        {
            return View();
        }

        //GET
        public ActionResult Contact()
        {
            //This represents the new wicked awesome feature that we added 

            ViewBag.Title = "Contact Us";
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //POST - sending our data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactViewModel contact)
        {
            //check the contact object for validity
            if (ModelState.IsValid)
            {
                //Create a body for the email (words)
                string body = string.Format($"Name: {contact.Name}<br />Email: {contact.Email}" +
                    $"<br />Subject: {contact.Subject}<br /><br />{contact.Message}");

                //Create and configure the Mail message (letter)
                MailMessage msg = new MailMessage("admin@scottiez.com", //from
                    "ziggish@att.net", //where we are sending to
                    contact.Subject, //subject of the message
                    body);

                //Configure the Mail message object (envelope)
                msg.IsBodyHtml = true; //body of the message is HTML
                                       //msg.cc.Add("ziggish@att.net");  sends a carbon copy
                                       //msg.Bcc.Add("ziggish@att.net"); //send a blind carbon copy so that no one knows that you got a CC
                msg.Priority = MailPriority.High;
                //create and configure the SMTP client  ... Standard Mail Transfer Protocol (mail carrier)
                SmtpClient client = new SmtpClient("mail.scottiez.com");  //mail person
                client.Credentials = new NetworkCredential("Admin@scottiez.com", "P@ssw0rd");
                client.Port = 8889;
                //the original is 25, but in case that port is being blocked, you can change the port number here

                //send the email
                using (client)
                {
                    try
                    {
                        client.Send(msg); //sending the MailMessage object
                    }
                    catch
                    {
                        ViewBag.ErrorMessage = "There was an error sending your message, please email admin@lindleyjon.com";
                        return View();
                    }
                }//using            

                //send the user to the ContactConfirmation View
                //pass the contact object with it 
                return View("ConfirmPartial", contact);

            }//if
            else
            {
                return View(); //the model isn't valid, return to form 
            }//else

        }



    }
}
