using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlinTuriCab.Models;

namespace AlinTuriCab.Controllers
{
    public class DefaultsController : Controller
    {
        CabDataContext db = new CabDataContext();

        [HttpPost]
        public JsonResult GetLocationsByCategory(string category)
        {
            return Json(db.Locations.Where(location => location.Category == category).Select(location => new
            {
                name = location.Name,
                address = location.Address,
                postcode = location.PostCode
            }));
        }

        [HttpPost]
        public ActionResult GetDialingCodes()
        {
            return View();
        }
    }
}
