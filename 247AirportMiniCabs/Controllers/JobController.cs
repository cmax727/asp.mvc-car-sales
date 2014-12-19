using System;
using System.Linq;
using System.Web.Mvc;
using AlinTuriCab.Models;
using AlinTuriCab.Helpers;

namespace AlinTuriCab.Controllers
{
    public class JobController : Controller
    {
        CabDataContext db = new CabDataContext();

        public ActionResult Index(string token)
        {
            var job = db.Jobs.FirstOrDefault(j => j.Token == token);
            if (job == null) return Redirect("/");

            return View(job);
        }

        [HttpPost]
        public ActionResult New()
        {
            var form = Request.Form;

            string passengerName = form["passengerName"],
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
                   returnTimeString = form["returnTime"];

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
            if (!string.IsNullOrWhiteSpace(returnTimeString)) DateTime.TryParse(returnTimeString, out returnTime);
            
        GoBack:
            var token = RandomNumbers.GetRandomNumbers();
            if (db.Jobs.Any(j => j.Token == token)) goto GoBack;

            var job = new Job
                          {
                              Name = passengerName,
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
                              ReturnTime = returnTime
                          };

            db.Jobs.InsertOnSubmit(job);
            db.SubmitChanges();
            
            return Redirect("/Job/?token=" + token);
        }

        [HttpPost]
        public JsonResult Update(string field, string token, string value)
        {
            var editJob = db.Jobs.FirstOrDefault(job => job.Token == token);
            if (editJob == null) return Json(false);

            if (field == "pick-from") editJob.PickFrom = value;
            if (field == "drop-to") editJob.DropTo = value;
            if (field == "pickup-time")
            {
                DateTime pickupTime;
                DateTime.TryParse(value, out pickupTime);
                editJob.PickupTime = pickupTime;
            }
            if (field == "fare") editJob.Fare = value;

            db.SubmitChanges();
            return Json(true);
        }
    }
}
