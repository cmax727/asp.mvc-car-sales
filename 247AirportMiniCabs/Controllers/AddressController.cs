using System.Linq;
using System.Web.Mvc;
using AlinTuriCab.Models;
using System.Web.WebPages;
using AlinTuriCab.Helpers;

namespace AlinTuriCab.Controllers
{
    public class AddressController : Controller
    {
        readonly PostCodesDataContext db = new PostCodesDataContext();
        public AddressController()
        {
            db.ObjectTrackingEnabled = false;
        }

        [HttpPost]
        public JsonResult Search(string term)
        {
            if (term.IsEmpty()) return Json(db.UKAddresses.Take(15).Select(a => new
            {
                p = a.PCUnit,
                s = a.Street,
                d = a.PCDistrict
            }));

            term = term.ToLower();

            var addresses = db.UKAddresses.Where(a => a.PCUnit != null && a.PCUnit.ToLower().StartsWith(term.ToLower()));

            return Json(addresses.Select(a => new
            {
                p = a.PCUnit,
                s = a.Street,
                d = a.PCDistrict
            }));
        }

        [HttpPost]
        public JsonResult NoPostcode(string postcode, string street)
        {
            var subject = "New postcode found: " + postcode;
            var body = "Sir, it seems that following postcode not exists in our database:<br /><br />";
            body += "Postcode: " + postcode;
            body += "<br />";
            body += "Street: " + street;
            new CabDataContext().SendEmailToAdmin(subject, body);

            return Json(true);
        }
    }
}