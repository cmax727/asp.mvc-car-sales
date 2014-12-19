using System.Linq;
using System.Web.Mvc;
using AlinTuriCab.Helpers;
using AlinTuriCab.Models;

namespace AlinTuriCab.Controllers
{
    public class AccountController : Controller
    {
        private readonly CabDataContext _db = new CabDataContext();

        public ActionResult Login()
        {
            if (LoginHelper.Client != null) return Redirect("/Client/");

            Response.Redirect("/login.html");
            return View();
        }

        [HttpPost]
        public JsonResult Login(string email, string password)
        {
            if (_db.Clients.Any(c => c.Email == email))
            {
                var client = _db.Clients.FirstOrDefault(c => c.Email == email);
                if (client == null) return Json(false);

                if (!client.IsConfirmed) return Json(new { error = "Please confirm your account. We have sent you a confirmation token at: <a href=\"mailto:" + client.Email + "\">" + client.Email + "</a><br /<br />Want to resend confirmation token? <a href=\"/resend-confirmation-token.html\"> Click here </a>." });

                if (BCrypt.CheckPassword(password, client.Password))
                {
                    client.Token.AddCookie();
                    _db.ValidateCookie();

                    return Json(true);
                }
            }

            return Json(false);
        }

        public ActionResult Logout()
        {
            if (LoginHelper.Client != null) LoginHelper.Client.Token.RemoveCookie();

            LoginHelper.Client = null;

            return RedirectToAction("Login");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Logout(string fake =  null)
        {
            if (LoginHelper.Client != null) LoginHelper.Client.Token.RemoveCookie();

            LoginHelper.Client = null;

            return Json(true);
        }

        public ActionResult NewClientAccount(string name)
        {
            ViewBag.Name = name.GetValidatedString();
            return View();
        }

        public ActionResult SignUp()
        {
            return Redirect("/open-an-account.html");
        }

        [HttpPost]
        public JsonResult IsLoggedIn()
        {
            return Json(LoginHelper.Client != null);
        }
    }
}
