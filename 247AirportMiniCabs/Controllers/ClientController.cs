using System;
using System.Linq;
using System.Web.Mvc;
using AlinTuriCab.Models;
using AlinTuriCab.Helpers;
using System.Web.WebPages;

namespace AlinTuriCab.Controllers
{
    public class ClientController : Controller
    {
        readonly CabDataContext db = new CabDataContext();

        public ClientController() { db.ValidateCookie(); }

        [HttpPost]
        public JsonResult New()
        {
            var form = Request.Form;

            string 
               name = form["name"],
               password = form["password"],
               email = form["email"],
               address = form["address"],
               postalCode = form["postalCode"],
               state = form["state"],
               city = form["city"],
               country = form["country"],
               cellPhone = form["cellPhone"],
               subscribe = form["subscribe"];

            if (db.Clients.Any(a => a.Email == email)) return Json(new { error = "This email address is being used by someone else. Please pick a unique email address." });

        GoBack:
            var token = RandomNumbers.GetRandomNumbers();
            if (db.Clients.Any(c => c.Token == token)) goto GoBack;

        BackAgain:
            var confirmationToken = RandomNumbers.GenerateCoupen();
            if (db.Clients.Any(c => c.ConfirmationToken == confirmationToken)) goto BackAgain;

            var client = new Client
            {
                Name = name,
                Password = BCrypt.HashPassword(password, BCrypt.GenerateSalt(8)),
                Email = email.ToLower(),
                Address = address,
                PostalCode = postalCode,
                State = state,
                MobilePhone = cellPhone,
                HomePhone = "",
                Token = token,
                City = city,
                Country = country,
                SubscribeNewsletter = subscribe == "1",
                IsConfirmed = false,
                ConfirmationToken = confirmationToken,
                RegistrationDate = UKTime.Now
            };

            try
            {
                db.Clients.InsertOnSubmit(client);
                db.SubmitChanges();

                client.SendEmail(db.Site());

                return Json(new
                {
                    name = client.Name,
                    cell = client.MobilePhone,
                    email = client.Email,
                    landline = client.HomePhone
                });
            }
            catch
            {
                return Json(new
                {
                    error = "Currently we're unable to send you a confirmation code via email. We apologize for this inconvenience.\n\n"
                    + "Our admin is being alerted about this situation. He'll contact you soon.\n\n"
                    + "Please check your email 10 minutes later.",
                    isEmailError = true
                });
            }
        }

        [HttpPost]
        public JsonResult Verify(string field, string value, bool confirmedClientsOnly = false)
        {
            Client client;
            if (field == "email")
            {
                client = db.Clients.FirstOrDefault(c => c.Email == value && (confirmedClientsOnly ? c.IsConfirmed : (c.IsConfirmed || !c.IsConfirmed)));
                if (client != null) return Json(new { error = "This email address is already been used. Please pick a unique email address. \n\nIf it is your email: send a direct message to admin via contact page to complain." });
            }

            if (field == "phone")
            {
                client = db.Clients.FirstOrDefault(c => (c.MobilePhone == value || c.HomePhone == value) && (confirmedClientsOnly ? c.IsConfirmed : (c.IsConfirmed || !c.IsConfirmed)));
                if (client != null) return Json(new { error = "This phone number is already been used.\n\nIf it is your phone number: send a direct message to admin via contact page to complain." });
            }

            Job job = null;
            // Now validating if email address already booked some jobs anonymously
            if (field == "email") job = db.Jobs.FirstOrDefault(j => j.Email == value);
            if (field == "phone") job = db.Jobs.FirstOrDefault(j => j.CellNumber == value || j.LandLine == value);

            if (job != null)
            {
                return Json(new
                {
                    name = job.Name,
                    email = job.Email,
                    cell = job.CellNumber,
                    landline = job.LandLine,
                    total = db.Jobs.Count(j => field == "email" ? (j.Email == value) : (j.CellNumber == value || j.LandLine == value)),
                    alreadyBooked = true
                });
            }

            return Json(true);
        }

        public ActionResult Confirm(string token)
        {
            ViewBag.Token = token;
            return View(db);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Confirm(string confirmationToken, string password)
        {
            var client = db.Clients.FirstOrDefault(c => c.ConfirmationToken == confirmationToken);
            if (client == null || BCrypt.CheckPassword(password, client.Password) == false) return Json(false);

            client.IsConfirmed = true;
            client.RegistrationDate = UKTime.Now; // confirmation date!

            db.SubmitChanges();

            client.ClientConfirmationNotification(db);

            // ---- adding cookies

            client.Token.AddCookie();
            db.ValidateCookie();

            db.SendEmailToAdmin("Confirmed Account", client.Name + " ( " + client.Email + " ) confirmed his account.");

            var discountToken = db.GetDiscountToken("5");

            return Json(new
            {
                name = client.Name,
                cell = client.MobilePhone,
                email = client.Email,
                landline = client.HomePhone,
                discount = discountToken
            });
        }

        [ValidateRequest]
        public ActionResult Index()
        {
            return View(db);
        }

        [HttpPost]
        public JsonResult UpdateAccount(string field, string value)
        {
            if (field.IsEmpty()) return Json(false);

            var client = db.Clients.FirstOrDefault(c => c.Token == LoginHelper.Client.Token);
            if (client == null) return Json(false);

            if (field == "address") client.Address = value;
            if (field == "state") client.State = value;
            if (field == "landline") client.HomePhone = value;
            if (field == "company") client.Company = value;

            if (value.IsEmpty() && (field == "name" || field == "email" || field == "mobile" || field == "password" || field == "country" || field == "city" || field == "postcode")) return Json(false);

            if (!value.IsEmpty())
            {
                if (field == "name") client.Name = value;
                if (field == "email") client.Email = value;
                if (field == "mobile") client.MobilePhone = value;
                if (field == "password") client.Password = BCrypt.HashPassword(value, BCrypt.GenerateSalt(8));

                if (field == "city") client.City = value;
                if (field == "postcode") client.PostalCode = value;
                if (field == "country") client.Country = value;

                if (field == "newsletter") client.SubscribeNewsletter = value == "1";
            }

            db.SubmitChanges();

            return Json(true);
        }

        [ValidateRequest]
        public ActionResult Edit(string id)
        {
            var job = db.Jobs.FirstOrDefault(j => j.Token == id);
            if (job == null)
            {
                if (Request.UrlReferrer != null)
                {
                    var referrer = Request.UrlReferrer.AbsolutePath;
                    if (referrer.IsEmpty()) referrer = "/";
                    Redirect(referrer);
                }
            }

            return View(job);
        }

        [ValidateRequest, HttpPost]
        public JsonResult EditJob(
                                    string jobToken,
                                    string customerName,
                                    string cellNumber,
                                    string landLine,
                                    string email,
                                    string pickFrom,
                                    string dropTo,
                                    string pickupTime,
                                    string returnTime,
                                    int numberOfPassengers,
                                    string vehicleType,
                                    int childSeats,
                                    int babySeats,
                                    int luggage,
                                    int handLag,
                                    string payAs,
                                    string additionalInfo,
                                    string journeyType,
                                    string stops,
                                    string passengerName,
                                    string passengerCell)
        {
            var job = db.Jobs.FirstOrDefault(j => j.Token == jobToken);
            if (job == null) return Json(false);

            if (jobToken.IsEmpty()
                || customerName.IsEmpty()
                || cellNumber.IsEmpty()
                || email.IsEmpty()
                || pickFrom.IsEmpty()
                || dropTo.IsEmpty()
                || pickupTime.IsEmpty()
                || vehicleType.IsEmpty()
                || payAs.IsEmpty()
                || journeyType.IsEmpty()
                )
            {
                return Json(false);
            }

            job.Name = customerName;
            job.CellNumber = cellNumber;
            job.LandLine = landLine;

            job.Email = email;

            job.PickFrom = pickFrom;
            job.DropTo = dropTo;

            DateTime pickupDateTime; DateTime.TryParse(pickupTime, out pickupDateTime);
            job.PickupTime = pickupDateTime;

            DateTime returnDateTime;
            if (returnTime.IsEmpty())
            {
                returnDateTime = UKTime.Now;
                job.ReturnTime = returnDateTime;
            }
            else
            {
                DateTime.TryParse(returnTime, out returnDateTime);
                job.ReturnTime = returnDateTime;
            }

            job.NumberOfPassengers = numberOfPassengers;
            job.VehicleToken = vehicleType;

            job.ChildSeats = childSeats;
            job.BabySeats = babySeats;
            job.Luggage = luggage;
            job.HandLag = handLag;

            job.PaidAs = payAs;

            job.AdditionalInfo = additionalInfo.GetValidatedString();

            job.JourneyType = journeyType;
            job.Stops = stops;

            job.PassengerName = passengerName;
            job.PassengerCell = passengerCell;

            db.SubmitChanges();

            var message = job.Name + " edited his job. Please count fare and send him direct email containg fares info.";
            db.NotifyOperator(JobType.JobError, message, job.Token);
            db.SendEmailToAdmin("Booking Edited", message);

            return Json(true);
        }

        [HttpPost, ValidateRequest]
        public ActionResult ExpandBooking(string token)
        {
            ViewBag.Token = token;
            return View(db);
        }

        [ValidateRequest, HttpPost]
        public JsonResult DirectMessage(string to, string type, string message)
        {
            Back:
            string uniqueToken = RandomNumbers.GetRandomNumbers(8);
            if (db.Notifications.Any(n => n.Token == uniqueToken)) goto Back;

            var client = LoginHelper.Client;
            new Notification
            {
                Sender = client.Token,
                SentAt = UKTime.Now,
                Receiver = to,
                Type = type,
                Message = message.GetValidatedString(),
                Status = "New",
                Token = uniqueToken
            }.Send(db);

            db.SendEmailToAdmin("Direct message: type: " + type + " to: " + to, message);

            return Json(true);
        }

        [ValidateRequest, HttpPost]
        public ActionResult GetClientMessages()
        {
            return View(db);
        }

        [ValidateRequest, HttpPost]
        public ActionResult GetClientNotifications()
        {
            return View(db);
        }

        [ValidateRequest, HttpPost]
        public ActionResult GetLatestJob()
        {
            return View(db);
        }

        [ValidateRequest, HttpPost]
        public ActionResult GetClientJobs(int skip, int take)
        {
            ViewBag.Skip = skip;
            ViewBag.Take = take;
            return View(db);
        }

        [ValidateRequest, HttpPost]
        public ActionResult GetNumberOf(string vehicleToken)
        {
            var vehicle = db.Vehicles.FirstOrDefault(v => v.Token == vehicleToken);
            if (vehicle == null) return Json(false);

            ViewBag.VehicleToken = vehicleToken;

            return View(db);
        }

        [ValidateRequest, HttpPost]
        public JsonResult UpdateNotification(string token, string status)
        {
            var notification = db.Notifications.FirstOrDefault(n => n.Token == token);
            if (notification == null || status.IsEmpty()) return Json(false);
            
            notification.Status = status;
            db.SubmitChanges();

            return Json(true);
        }

        [HttpPost]
        public JsonResult ResendConfirmationCode(string email)
        {
            var client = db.Clients.FirstOrDefault(c => c.Email == email);
            if (client == null) return Json(new { error = "No such email registered with us!" });

            if (client.IsConfirmed) return Json(new
            {
                error = "You already confirmed your account."
            });

        BackAgain:
            var confirmationToken = RandomNumbers.GenerateCoupen();
            if (db.Clients.Any(c => c.ConfirmationToken == confirmationToken)) goto BackAgain;

            client = db.Clients.FirstOrDefault(c => c.Token == client.Token);
            if (client == null) return Json(false);
            
            try
            {
                client.ConfirmationToken = confirmationToken;
                db.SubmitChanges();

                client.ClientResentConfirmationCode(db);

                client.SendEmail(db.Site());

                return Json(true);
            }
            catch
            {
                return Json(new
                {
                    error = "Currently we're unable to send you a confirmation code via email. We apologize for this inconvenience.\n\n"
                    + "Our admin is being alerted about this situation. He'll contact you soon.\n\n"
                    + "Please check your email 10 minutes later.",
                    isEmailError = true
                });
            }
        }


        [HttpPost]
        public JsonResult ForgotPassword(string email)
        {
            var user = db.Clients.FirstOrDefault(a => a.Email == email);
            if (user == null) return Json("No such email has been registered in our site.<br /><br />Please make sure that you entered a valid email address?<br /><br />You can directly contact us via our <a href=\"/contact-us.html\">Contact</a> section.");

            try
            {
                user.ForgotPasswordToken = RandomNumbers.GenerateCoupen();
                db.SubmitChanges();

                user.SendChangePasswordEmail(db.Site());
                return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }

        public ActionResult ChangePassword(string forgot)
        {
            ViewBag.Token = forgot;
            return View(db);
        }

        [ValidateRequest]
        public ActionResult _ChangePassword()
        {
            var client = db.Clients.FirstOrDefault(c => c.Token == LoginHelper.Client.Token);
            if (client == null) return Redirect("/logout");

            if (client.ForgotPasswordToken.IsEmpty())
            {
                client.ForgotPasswordToken = RandomNumbers.GenerateCoupen();
                db.SubmitChanges();
            }

            return Redirect("/confirm-password.html#" + client.ForgotPasswordToken);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ChangePassword(string token, string password, string oldPassword = null, bool mustCheckOld = false)
        {
            var client = db.Clients.FirstOrDefault(c => c.ForgotPasswordToken == token);
            if (client == null) return Json(false);

            if (mustCheckOld && BCrypt.CheckPassword(oldPassword, client.Password) == false) return Json(false);

            client.Password = BCrypt.HashPassword(password, BCrypt.GenerateSalt(8));
            client.ForgotPasswordToken = null;

            db.SubmitChanges();

            return Json(true);
        }

        [HttpPost]
        public JsonResult GetBonusToken()
        {
            var discount = db.Discounts.FirstOrDefault(d => !d.IsExpired && d.ValidTill > UKTime.Now && d.ClientToken == LoginHelper.Client.Token);
            if (discount == null) return Json(false);
            return Json(discount.Token);
        }

        [ValidateRequest]
        public ActionResult TempJobEdit(string id)
        {
            if (id.IsEmpty()) return Redirect("/");
            ViewBag.ID = id;
            return View(db);
        }

        [HttpPost, ValidateRequest]
        public JsonResult TempJobEdit(string id, string pickup, string dropoff, string pickupTime, string message)
        {
            if (id.IsEmpty()) return Json(false);

            var job = db.Jobs.FirstOrDefault(j => j.Token == id);
            if (job == null) return Json(false);

            var body = "";
            if (!pickup.IsEmpty()) body += "Pickup: " + pickup + "<br /><br />";
            if(!dropoff.IsEmpty()) body += "Dropoff: " + dropoff + "<br /><br />";
            if (!pickupTime.IsEmpty()) body += "Pickup time: " + pickupTime + "<br /><br />";
            if (!message.IsEmpty()) body += "Message: " + message;

            if (!body.IsEmpty())
            {
                db.SendEmailToAdmin("Job edited by " + LoginHelper.Client.Name, body);
                return Json(true);
            }
            return Json(false);
        }

        [HttpPost, ValidateRequest]
        public JsonResult CancelBooking(string id)
        {
            var job = db.Jobs.FirstOrDefault(j => j.Token == id);
            if (job == null) return Json(false);

            var body = LoginHelper.Client.Name +  " ( " + LoginHelper.Client.Email + " | " + LoginHelper.Client.MobilePhone + " ) want to cancel his booking number: " + job.JobNumber;
            db.SendEmailToAdmin("Cancel Booking Request", body);

            return Json(true);
        }

        [HttpPost, ValidateRequest]
        public JsonResult MarkAllRead()
        {
            var notifications = db.Notifications.Where(n => n.Receiver == LoginHelper.Client.Token && n.Status == "New");
            foreach (var notification in notifications)
            {
                notification.Status = "Read";
            }
            db.SubmitChanges();
            return Json(true);
        }
    }
}