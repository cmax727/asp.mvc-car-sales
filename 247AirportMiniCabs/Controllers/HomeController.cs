using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlinTuriCab.Models;
using AlinTuriCab.Helpers;
using System.Web.Helpers;
using System.Web.WebPages;

namespace AlinTuriCab.Controllers
{
    public class HomeController : Controller
    {
        CabDataContext db = new CabDataContext();
        public ActionResult Index()
        {
            db.ValidateCookie();
            if (LoginHelper.Client != null) return Redirect("/Client/");

            return View();
        }

        [HttpPost]
        public JsonResult ContactUs(string message, string email = null, string name = null, string phone = null)
        {
            var site = db.Site();

            var body = "";
            if (!name.IsEmpty()) body += "<strong>Name:</strong> " + name + "<br /><br />";
            if (!name.IsEmpty()) body += "<strong>Email:</strong> " + email + "<br /><br />";
            if (!name.IsEmpty()) body += "<strong>Phone:</strong> " + phone + "<br /><br />";

            body += "<strong>Message:</strong><br /><br /><blockquote>" + message + "</blockquote><br /><br />";
            body += "<strong>Sent from:</strong> " + site.Name + " ( " + site.URL + " )";

            var subject = "Contact message from " + site.URL;

            try
            {
                db.NotifyAdmin("Contact", body);
                db.SendEmailToAdmin(subject, body);
                return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult TermsAndConditions()
        {
            return View();
        }

        public ActionResult SafetyPolicy()
        {
            return View();
        }

        public ActionResult CancellationRefundPolicy()
        {
            return View();
        }

        public ActionResult DriverJob()
        {
            return View();
        }

        public ActionResult ChauffeurCars()
        {
            return View(db);
        }

        public ActionResult Testimonials()
        {
            return View();
        }

        public ActionResult Services()
        {
            return View();
        }
    }
}