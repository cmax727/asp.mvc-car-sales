using System.Web.Mvc;
using AlinTuriCab.Models;
using AlinTuriCab.Helpers;

namespace AlinTuriCab.Controllers
{
    public class FeedbackController : Controller
    {
        readonly CabDataContext _db = new CabDataContext();

        public ActionResult Index(bool isFullPage = true)
        {
            return LoginHelper.Client == null ? Redirect("/logout") : View();
        }

        [HttpPost]
        public JsonResult Add(
            string name, string email,
            string dateOfBooking,
            bool isFirstBooking, 
            string scoreForUs,
            string scoreForCustomerService,

            string scoreForService, 
            string scoreForJourney, 
            string scoreForOnlineBooking, 
            string defineService, 
            string bookUsAgain
            )
        {
            var feedback = new Feedback
            {
                Name =  name,
                Email =  email,
                DateOfBooking =  dateOfBooking,
                FeedbackDate = UKTime.Now,
                IsFirstBooking = isFirstBooking,
                ScoreForUs =  scoreForUs,
                ScoreForCustomerService = scoreForCustomerService,
                ScoreForJourney = scoreForJourney,
                ScoreForOnlineBooking = scoreForOnlineBooking,

                DefineOurService = defineService.GetValidatedString(),
                BookUsAgain = bookUsAgain
            };

            _db.Feedbacks.InsertOnSubmit(feedback);

            _db.SubmitChanges();

            return Json(true);
        }

        [ValidateRequest]
        public ActionResult GetForClient()
        {
            return View(_db);
        }

        public ActionResult View(bool isFullpage = true)
        {
            ViewBag.IsFullPage = isFullpage;
            return View(_db);
        }
    }
}
