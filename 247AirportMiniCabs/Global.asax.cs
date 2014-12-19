using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace AlinTuriCab
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("book", "book", new { controller = "Main", action = "MakeABooking" });
            routes.MapRoute("logout", "logout", new { controller = "Account", action = "Logout" });

            routes.MapRoute("signup", "signup", new { controller = "Account", action = "SignUp" });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Main", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        void Application_BeginRequest(object sender, EventArgs ex)
        {
            if (!Helpers.StaticHelper.IsOnline || Request.Url.Authority.StartsWith("www")) return;

            var url = string.Format("{0}://www.{1}{2}",
                                       Request.Url.Scheme,
                                       Request.Url.Authority,
                                       Request.Url.PathAndQuery);

            Response.RedirectPermanent(url, true);
        }
    }
}