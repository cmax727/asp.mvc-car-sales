using System.Web.Mvc;
using AlinTuriCab.Models;

namespace AlinTuriCab.Controllers
{
    public class PaymentController : Controller
    {
        public ActionResult Index()
        {
            Global global = new Global();
            global.Application_Start();


            ViewBag.Message = "Welcome to Payment Sense Direct payment";
            Helper _helper = new Helper();
            ViewData["ddCountries"] = new SelectList(_helper.CountryList, "Name", "Code");
            ViewData["months"] = new SelectList(_helper.GetMonths, "MonthName", "MonthValue");
            ViewData["startYears"] = new SelectList(_helper.GetStartDateYear, "YearValue", "Year");
            ViewData["expiryYears"] = new SelectList(_helper.GetExpiryDateYear, "YearValue", "Year");

            PaymentModel _model = new PaymentModel();
            ViewData["_model"] = _model;
            return View(_helper);
        }

        public ActionResult About()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(PaymentModel model)
        {
            PaymentModel _model = new PaymentModel();
            PaymentServiceAttributes _attributes = new PaymentServiceAttributes();

            _attributes.ddExpiryDateMonth = Request.Form["ddExpiryDateMonth"];
            _attributes.ddExpiryDateYear = Request.Form["ddExpiryDateYear"];
            _attributes.ddStartDateMonth = Request.Form["ddStartDateMonth"];
            _attributes.ddStartDateYear = Request.Form["ddStartDateYear"];
            _attributes.tbCardName = Request.Form["tbCardName"];
            _attributes.tbCardNumber = Request.Form["tbCardNumber"];
            _attributes.tbIssueNumber = Request.Form["tbIssueNumber"];
            _attributes.tbCV2 = Request.Form["tbCV2"];
            _attributes.ddCountries = Request.Form["ddCountries"];
            _attributes.tbAddress1 = Request.Form["tbAddress1"];
            _attributes.tbAddress2 = Request.Form["tbAddress2"];
            _attributes.tbAddress3 = Request.Form["tbAddress3"];
            _attributes.tbAddress4 = Request.Form["tbAddress4"];
            _attributes.tbCity = Request.Form["tbCity"];
            _attributes.tbState = Request.Form["tbState"];
            _attributes.tbPostCode = Request.Form["tbPostCode"];
            _attributes.hfAmount = Request.Form["hfAmount"];
            _attributes.hfCurrencyISOCode = Request.Form["hfCurrencyISOCode"];
            _attributes.hfOrderID = Request.Form["hfOrderID"];
            _attributes.hfOrderDescription = Request.Form["hfOrderDescription"];
            _attributes.UserAgent = Request.UserAgent;
            _attributes.UserHostIPAddress = "127.0.0.1"; // This is actually Request.HostUserHostAddress. If you use HostUserHostAddress make sure that it is in valid format

            _model = _model.ProcessDirectCardPayment(_attributes);

            ViewData["_model"] = _model;

            Helper _helper = new Helper();
            ViewData["ddCountries"] = new SelectList(_helper.CountryList, "Name", "Code");
            ViewData["months"] = new SelectList(_helper.GetMonths, "MonthName", "MonthValue");
            ViewData["startYears"] = new SelectList(_helper.GetStartDateYear, "Year", "YearValue");
            ViewData["expiryYears"] = new SelectList(_helper.GetExpiryDateYear, "Year", "YearValue");

            return View();
        }
    }
}
