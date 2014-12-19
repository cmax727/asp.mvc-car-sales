/* Muaz Khan – http://muaz-khan.blogspot.com */

using System;
using System.Web;
using AlinTuriCab.Models;
using System.Linq;

namespace AlinTuriCab.Helpers
{
    public static class CookieExtensions
    {
        const string CookieName = "TaxiCab";
        public static void RemoveCookie(this string loginToken)
        {
            var request = HttpContext.Current.Request;
            var responce = HttpContext.Current.Response;

            if (request.Cookies[CookieName] == null) return;
            
            responce.Cookies.Remove(CookieName);

            var cookie = new HttpCookie(CookieName)
            {
                Expires = UKTime.Now.AddDays(-10)
            };

            responce.Cookies.Add(cookie);
            var httpCookie = responce.Cookies[CookieName];
            if (httpCookie != null) httpCookie.HttpOnly = true;
        }

        public static void AddCookie(this string loginToken)
        {
            AddOrRemove(loginToken.ToText());
        }
        private static void AddOrRemove(string cookieValue)
        {
            var request = HttpContext.Current.Request;
            var responce = HttpContext.Current.Response;

            if (!request.Browser.Cookies) return;

            if (request.Cookies[CookieName] == null)
            {
                cookieValue.AddCookie(responce);
            }
            else if (request.Cookies[CookieName] != null)
            {
                responce.Cookies.Remove(CookieName);
                cookieValue.AddCookie(responce);
            }
        }
        private static void AddCookie(this string value, HttpResponse responce)
        {
            var cookie = new HttpCookie(CookieName)
            {
                Value = value,
                Expires = UKTime.Now.AddYears(1)
            };
            responce.Cookies.Add(cookie);
            var httpCookie = responce.Cookies[CookieName];
            if (httpCookie != null) httpCookie.HttpOnly = true;
        }

        public static void ValidateCookie(this CabDataContext db, int index = 0)
        {
            var request = HttpContext.Current.Request;
            if (!request.Browser.Cookies || request.Cookies.Count <= 0 || request.Cookies[CookieName] == null)
            {
                LoginHelper.InvalidLoginRequest();
                return;
            }

            string cookieValue = request.Cookies[CookieName].Value;

            if (request.Cookies[CookieName] != null && cookieValue != null)
            {
                var loginToken = cookieValue;
                loginToken = loginToken.ToNumbers();

                if (db.Clients.Any(c => c.Token == loginToken)) 
                    db.ValidateClientRequest(loginToken);
                else LoginHelper.InvalidLoginRequest();
            }
            else LoginHelper.InvalidLoginRequest();
        }
    }
}