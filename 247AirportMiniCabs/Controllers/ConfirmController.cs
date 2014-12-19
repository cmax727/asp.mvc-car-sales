using System.Linq;
using System.Web.Mvc;
using AlinTuriCab.Models;
using AlinTuriCab.Helpers;

namespace AlinTuriCab.Controllers
{
    public class ConfirmController : Controller
    {
        public ActionResult Index(string job = null, string client = null, string forgot = null)
        {
            if (client != null) return RedirectToAction("Confirm", "Client", new { token = client });
            if (forgot != null) return RedirectToAction("ChangePassword", "Client", new { forgot });

            var db = new CabDataContext();
            var bookinng = db.Jobs.FirstOrDefault(j => j.ConfirmationToken == job);

            if (bookinng == null || bookinng.IsConfirmed) return Redirect("/");

            bookinng.IsConfirmed = true;
            db.SubmitChanges();

            bookinng.JobConfirmationNotification(db);

            db.SendEmailToAdmin("Booking Confirmed", "Booking confirmed by " + bookinng.Name + " ( " + bookinng.Email + " ).", StaticHelper.JobURI);


            var notifications = db.Notifications.Where(n => n.Receiver == bookinng.ClientToken && n.Type == JobType.SentCodeForJob);
            foreach (var notification in notifications)
            {
                notification.Status = "Read";

                db.SubmitChanges();
            }

            return View(bookinng);
        }

        public ActionResult Preview(string token, string type)
        {
            var db = new CabDataContext();
            var booking = db.Jobs.FirstOrDefault(j => j.ConfirmationToken == token);

            if (booking == null) return Json("<h2>Booking not found.</h2>", JsonRequestBehavior.AllowGet);

            if (type == "confirm")
            { 
                booking.IsConfirmed = true;
                db.SubmitChanges();

                booking.JobConfirmationNotification(db);
                db.SendEmailToAdmin("Booking Confirmed", "Booking confirmed by " + booking.Name + " ( " + booking.Email + " ).");

                booking.SendBookingConfirmedEmail(db.Site());

                var notifications = db.Notifications.Where(n => n.Receiver == booking.ClientToken && n.Type == JobType.SentCodeForJob);
                foreach (var notification in notifications)
                {
                    notification.Status = "Read";
                    db.SubmitChanges();
                }
            }

            if (type == "cancel")
            {
                booking.IsConfirmed = false;
                db.SubmitChanges();

                var body = "A customer ( " + booking.Name + " : " + booking.CellNumber + " ) want to cancel his booking number #" + booking.JobNumber + " <a href=\"http://" + (db.Site().URL + "/confirm-booking.html#details=" + booking.ConfirmationToken) + "\">View Booking Details</a>";
                db.SendEmailToAdmin("Customer want to cancel his booking", body);
            }

            return View(booking);
        }

        [HttpPost]
        public JsonResult Confirm(string token, string type, string password, bool isEmail = false)
        {
            var db = new CabDataContext();

            switch (type)
            {
                case "job":
                    {
                        var booking = db.Jobs.FirstOrDefault(j => j.ConfirmationToken == token);

                        if (booking == null || booking.IsConfirmed)
                        {
                            db.NotifyOperator("Unknown Error", "A customer tried to confirm booking that not exists or deleted. Booking token provided is: " + token);
                            return Json("No such booking found!");
                        }

                        if (isEmail && booking.Email != password) return Json("Invalid email or you've not one who booked the job!");
                        
                        if(!isEmail)
                        {
                            var client = LoginHelper.Client;
                            if (client == null || BCrypt.CheckPassword(password, client.Password) == false) return Json("Invalid password.");
                        }

                        booking.IsConfirmed = true;
                        db.SubmitChanges();

                        booking.JobConfirmationNotification(db);
                        db.SendEmailToAdmin("Booking Confirmed", "Booking confirmed by " + booking.Name + " ( " + booking.Email + " ).");

                        var site = db.Site();
                        var subject =booking.Name + ": Your booking confirmed successfully!";
                        var body = "Dear " + booking.Name + "!<br /><br />You booking number #" + booking.JobNumber + " confirmed successfully.";
                        new Live(site.BookingEmail, site).SendEmail(subject, body, booking.Email);

                        var notifications = db.Notifications.Where(n => n.Receiver == booking.ClientToken && n.Type == JobType.SentCodeForJob);
                        foreach (var notification in notifications)
                        {
                            notification.Status = "Read";
                            db.SubmitChanges();
                        }

                        return Json(true);
                    }
                case "change-password":
                    {
                        var client = db.Clients.FirstOrDefault(c => c.ForgotPasswordToken == token);
                        if (client == null) return Json(false);

                        client.Password = BCrypt.HashPassword(password, BCrypt.GenerateSalt(8));
                        client.ForgotPasswordToken = null;

                        db.SubmitChanges();

                        return Json(true);
                    }
                case "account":
                    {
                        var client = db.Clients.FirstOrDefault(c => c.ConfirmationToken == token);
                        if (client == null || BCrypt.CheckPassword(password, client.Password) == false) return Json(false);

                        client.IsConfirmed = true;
                        client.RegistrationDate = UKTime.Now;

                        db.SubmitChanges();

                        client.ClientConfirmationNotification(db);

                        client.Token.AddCookie();
                        db.ValidateCookie();

                        db.SendEmailToAdmin("Confirmed Account", client.Name + " ( " + client.Email + " ) confirmed his account.");

                        db.GetDiscountToken("5");

                        return Json(true);
                    }
            }

            return Json(false);
        }
    }
}
