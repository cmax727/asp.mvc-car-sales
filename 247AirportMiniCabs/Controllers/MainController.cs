using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using AlinTuriCab.Models;
using AlinTuriCab.Helpers;

namespace AlinTuriCab.Controllers
{
    public class MainController : Controller
    {
        readonly CabDataContext _db = new CabDataContext();
        public MainController()
        {
            _db.ValidateCookie();
        }

        public ActionResult Index()
        {
            
            Response.Redirect("/index.html");

            return View();
        }

        public ActionResult MakeABooking(string discount = null)
        {
            discount = discount.IsEmpty() ? "" : "#" + discount;

            Response.Redirect("/qoute.html" + discount);

            return View(_db);
        }

        [HttpPost]
        public ActionResult SelectTaxi(string pickFrom, string dropTo, string fromPostCode, string toPostCode, string selected, string journeyType, string distancePreview, string stops, string bonusToken = null)
        {
            ViewBag.PickFrom = pickFrom;
            ViewBag.DropTo = dropTo;

            ViewBag.FromPostCode = fromPostCode;
            ViewBag.ToPostCode = toPostCode;

            ViewBag.Selected = selected;

            ViewBag.JourneyType = journeyType;
            ViewBag.DistancePreview = distancePreview;

            var site = _db.Site();

            var fare = _db.Fares.FirstOrDefault(f => f.From == pickFrom && f.To == dropTo && f.SiteToken == site.Token) ??
                       _db.Fares.FirstOrDefault(f => f.From == dropTo && f.To == pickFrom && f.SiteToken == site.Token);

            if (fare == null)
                fare = _db.Fares.FirstOrDefault(f => f.From == fromPostCode && f.To == toPostCode && f.SiteToken == site.Token) ??
                         _db.Fares.FirstOrDefault(f => f.From == toPostCode && f.To == fromPostCode && f.SiteToken == site.Token);

            if (fare != null) ViewBag.Predefined = fare;

            return View(_db);
        }

        [HttpPost]
        public  ActionResult FinalizeBooking(string pickFrom, string dropTo, string vehicleType, string fare, string journeyType, string selected, string stops, string bonusToken = null, string airportCharges = null)
        {
            ViewBag.PickFrom = pickFrom;
            ViewBag.DropTo = dropTo;
            ViewBag.VehicleType = vehicleType;
            ViewBag.Fare = fare;
            ViewBag.JourneyType = journeyType;
            ViewBag.Stops = stops;
            ViewBag.BonusToken = bonusToken;

            selected = selected.Replace(",", "_____").Replace("----", ",");

            var splitted = selected.Split(',');
            string pickupSelected = splitted[0].Replace("_____", ","),
                dropoffSelected = splitted[1].Replace("_____", ",");


            //---------- Pickup relevant
            ViewBag.IsPickupPostCode = pickupSelected == "Post code";
            ViewBag.IsPickupAirport = pickupSelected == "Airport";

            ViewBag.FromType = pickupSelected;
            ViewBag.ToType = dropoffSelected;

            //---------- Drop-off relevant
            ViewBag.IsDropoffPostCode = dropoffSelected == "Post code";
            ViewBag.IsDropoffAirport = dropoffSelected == "Airport";

            ViewBag.Site = _db.Site();

            ViewBag.AirportCharges = airportCharges;

            return View();
        }

        [HttpPost]
        public ActionResult LastAction()
        {
            var form = Request.Form;

            ViewBag.CustomerName = form["customerName"];
            ViewBag.CellNumber = form["cellNumber"];
            ViewBag.LandLine = form["landLine"];
            ViewBag.Email = form["email"];
            
            ViewBag.Newspaper = form["newspaper"];
            ViewBag.Drink = form["drink"];

            ViewBag.Subscribe = form["subscribe"];

            string passengerName = form["passengerName"];
            if (!passengerName.IsEmpty())
            {
                ViewBag.IsDifferentPassenger = true;
                ViewBag.PassengerName = passengerName;
                ViewBag.PassengerCell = form["passengerCell"];
            }
            else ViewBag.IsDifferentPassenger = false;

            ViewBag.PickFrom = form["pickFrom"];
            ViewBag.DropTo = form["dropTo"];
            ViewBag.VehicleType = form["vehicleType"];

            ViewBag.Fare = form["fare"];

            ViewBag.PayAs = form["payAs"];
            ViewBag.AdditionalInfo = form["additionalInfo"].GetValidatedString();

            ViewBag.HandLag = form["handLag"];
            ViewBag.JourneyType = form["journeyType"];
            ViewBag.Stops = form["stops"];
            ViewBag.ReturnTime = form["returnTime"];

            ViewBag.NumberOfPassengers = form["numberOfPassengers"];
            ViewBag.ChildSeats = form["childSeats"];
            ViewBag.BabySeats = form["babySeats"];

            ViewBag.Luggage = form["luggage"];

            ViewBag.PickupTime = form["pickupTime"];
            ViewBag.BonusToken = form["bonusToken"];

            ViewBag.AirportCharges = form["airportCharges"];

            return View();
        }

        [HttpPost]
        public JsonResult ConfirmBooking()
        {
            var form = Request.Form;

            string customerName = form["customerName"],
                   cellNumber = form["cellNumber"],
                   pickFrom = form["pickFrom"],
                   dropTo = form["dropTo"],
                   vehicleType = form["vehicleType"],
                   email = form["email"],
                   payAs = form["payAs"],
                   additionalInfo = form["additionalInfo"],
                   fare = form["fare"],
                   landLine = form["landLine"],
                   handLag = form["handLag"],
                   journeyType = form["journeyType"],
                   stops = form["stops"],
                   returnTimeString = form["returnTime"],
                   passengerName = form["passengerName"],
                   passengerCell = form["passengerCell"],
                   bonusToken = form["bonusToken"];
            
            var clientToken = "None";
            if (LoginHelper.Client != null)
            {
                var client = LoginHelper.Client;
                clientToken = client.Token;
            }

            int numberOfPassengers;
            int.TryParse(form["numberOfPassengers"], out numberOfPassengers);

            int childSeats;
            int.TryParse(form["childSeats"], out childSeats);

            int babySeats;
            int.TryParse(form["babySeats"], out babySeats);

            int luggage;
            int.TryParse(form["luggage"], out luggage);

            int handLagNumber;
            int.TryParse(handLag, out handLagNumber);

            DateTime pickupTime; DateTime.TryParse(form["pickupTime"], out pickupTime);
            DateTime returnTime = UKTime.Now;
            if (!returnTimeString.IsEmpty()) DateTime.TryParse(returnTimeString, out returnTime);

        GoBack:
            var token = RandomNumbers.GetRandomNumbers();
            if (_db.Jobs.Any(j => j.Token == token)) goto GoBack;

        BackAgain:
            var confirmationToken = RandomNumbers.GenerateCoupen();
            if (_db.Jobs.Any(j => j.ConfirmationToken == confirmationToken)) goto BackAgain;

            const int bottleOfWater = 2;
            string newspaper = null;
            var vehicle = vehicleType.GetVehicle(_db);
            if (vehicle.Name == "Executive")
            {
                newspaper = form["newspaper"];
            }

            string drink = null;
            if (vehicle.Name == "Gold-Vip")
            {
                drink = form["drink"];
                newspaper = form["newspaper"];
            }

            int jobNumber = 1;
            if (_db.Jobs.Any()) jobNumber = (_db.Jobs.OrderByDescending(j => j.JobNumber).FirstOrDefault() ?? new Job { JobNumber = 0 }).JobNumber + 1;

            var site = _db.Site();

            var job = new Job
                {
                    Name = customerName,
                    PickFrom = pickFrom,
                    DropTo = dropTo,
                    PickupTime = pickupTime,
                    Fare = fare,
                    NumberOfPassengers = numberOfPassengers,
                    Luggage = luggage,
                    ClientToken = clientToken,
                    PaidAs = payAs,
                    Token = token,
                    VehicleToken = vehicleType,
                    ChildSeats = childSeats,
                    BabySeats = babySeats,
                    CellNumber = cellNumber,
                    Stops = stops,
                    DriverToken = "None",
                    Status = "Active",
                    Email = email,
                    AdditionalInfo = additionalInfo,
                    LandLine = landLine,
                    HandLag = handLagNumber,
                    JourneyType = journeyType,
                    ReturnTime = returnTime,

                    IsConfirmed = false,
                    ConfirmationToken = confirmationToken,

                    BottleOfWater = bottleOfWater,
                    Newspaper = newspaper,
                    Drink = drink,

                    JobNumber = jobNumber,

                    SiteToken = site.Token,
                    BookingFrom = site.Token,
                    BookingTime = UKTime.Now,
                    MinicabOffice = "",
                    BusinessAccountHolder = "",
                    CardTransactionCode = ""
                };

            if (!passengerName.IsEmpty())
            {
                job.PassengerName = passengerName;
                job.PassengerCell = passengerCell;
            }

            try
            {
                _db.Jobs.InsertOnSubmit(job);
                _db.SubmitChanges();

                string message;

                if (!bonusToken.IsEmpty())
                {
                    var discount =
                        _db.Discounts.FirstOrDefault(
                            d =>
                            d.Token == bonusToken && d.ClientToken == LoginHelper.Client.Token && !d.IsExpired &&
                            d.ValidTill >= UKTime.Now);
                    if (discount != null)
                    {
                        discount.JobToken = job.Token;
                        discount.IsExpired = true;
                        _db.SubmitChanges();
                    }
                    else
                    {
                        message = "Invalid bonus token ( " + bonusToken + " ) used for this job!";
                        _db.NotifyOperator(JobType.NewJob, message, job.Token);
                    }
                }

                message = "New job added by " + job.Name + " ( Cell: " + job.CellNumber + " | Email: " + job.Email + " )";
                _db.NotifyOperator(JobType.NewJob, message, job.Token);

                if (job.Fare.IsEmpty() || job.Fare == "0" || job.Fare == "NaN")
                {
                    message = "Unable to count fare for job added by " + job.Name + " ( Cell: " + job.CellNumber + " | Email: " + job.Email + " )";
                    _db.NotifyOperator(JobType.JobError, message, job.Token);
                    
                    _db.SendEmailToAdmin("Unable to Count Fare for a booking!", message);
                }
            }
            catch(Exception ex)
            {
                string errorMessage = "Unable to add booking for: " + job.Name + " ( Cell: " + job.CellNumber + " | Email: " + job.Email + " )";
                errorMessage += "<br />";
                errorMessage += "<br />Pick from: " + job.PickFrom;
                errorMessage += "<br />Drop to: " + job.DropTo;

                if (!job.Stops.IsEmpty())
                {
                    job.Stops = stops.Replace(",", "=====").Replace("-----", ",");
                    var splittedStops = stops.Split(',');
                    int index = 0;
                    foreach (var stop in splittedStops)
                    {
                        index++;
                        errorMessage += "<br />" + index + ". " + stop;
                    }
                }
                errorMessage += "<br />Pickup time: " + job.PickupTime;
                errorMessage += "<br />Journey type: " + job.JourneyType;
                errorMessage += "<br />Vehicle: " + job.VehicleToken;
                errorMessage += "<br />Fare: " + job.Fare;
                errorMessage += "<br /><br />Error Message:<br /><br /> " + ex;

                _db.NotifyOperator(JobType.JobError, errorMessage);
                _db.SendEmailToAdmin("Unable to add booking", errorMessage);

                const string message = "We are really sorry to say that something went wrong on our servers.<br />However, an automatic message has been sent to the administrator.<br />We will contact you soon via email or call.";

                if (LoginHelper.Client != null)
                {
                    LoginHelper.Client.NotifyClient(_db, "Something went wrong", message);
                }

                return Json(message);
            }

            //-------------------------------------------------------------------------------------
            //-------------------- Now sending email confirmation message
            //-------------------------------------------------------------------------------------

            try
            {
                //job.SendEmail(_db.Site(), airportCharges);
                //job.JobSentConfirmationCode(_db);

                return Json(
                    new
                        {
                            booked = true,
                            registered = LoginHelper.Client != null,
                            token = LoginHelper.Client != null ? job.ConfirmationToken : null
                        });
            }
            catch
            {
                string errorMessage = "Unable to send confirmation email to: " + job.Name + " ( Cell: " + job.CellNumber + " | Email: " + job.Email + " )";
                _db.NotifyOperator(JobType.JobError, errorMessage, job.Token);
                _db.SendEmailToAdmin("New Booking: Unable to send confirmation code!", errorMessage);

                var message = "We are unable to drop a confirmation code at your email: <a href=\"mailto:" + job.Email + "\">" + job.Email + "</a><br />An automatic email has been sent to the administrator.<br />We will send you confirmation code in a few minutes at your email address: <a href=\"mailto:" + job.Email + "\">" + job.Email + "</a>";

                if (LoginHelper.Client != null)
                {
                    LoginHelper.Client.NotifyClient(_db, "Unable to send confirmation code", message, job.Token);
                }
                return Json(message);
            }
        }
    }
}
