using System.Web.Helpers;
using AlinTuriCab.Models;
using System.Web.WebPages;
using System.Web;

namespace AlinTuriCab.Helpers
{
    public class Live
    {
        static Site _site;
        static string _email;

        public Live(string email, Site site = null)
        {
            _email = !email.IsEmpty() ? email : "booking@draculacars.co.uk";
            _site = _site != null ? site : new CabDataContext().Site();
        }

        internal string From
        {
            get { return _email; }
        }
        internal string Password
        {
            get { return _site.EmailPassword; }
        }
        internal string Email
        {
            get { return _email; }
        }
        internal int SmtpPort
        {
            get { return _site.EmailPort; }
        }
        internal string SmtpServer
        {
            get { return _site.EmailSMTP; }
        }
    }

    public static class EmailExtensions
    {
        public static void SendEmailToAdmin(this CabDataContext db, string subject, string body, string email = null)
        {
            var site = db.Site();
            new Live(email ?? "admin@draculacars.co.uk", site).SendEmail(subject, body, email ?? "admin@draculacars.co.uk");
        }

        public static string Website
        {
            get
            {
                var url = HttpContext.Current.Request.Url;
                return url.Scheme + "://" + url.Host + "/";
            }
        }

        public static void SendEmail(this Live live, string subject, string body, string email)
        {
            WebMail.From = live.From;
            WebMail.Password = live.Password;
            WebMail.SmtpPort = live.SmtpPort;
            WebMail.SmtpServer = live.SmtpServer;
            WebMail.UserName = live.Email;
            WebMail.EnableSsl = false;
            WebMail.SmtpUseDefaultCredentials = false;

            try
            {
                WebMail.Send(email, subject, body);
            }
            catch
            {}
        }

        public static void SendEmail(this Job job, Site site, string airportCharges = null)
        {
            string confirmationLink = site.URL + "/confirm-booking.html#confirm=" + job.ConfirmationToken;
            string detailsLink = site.URL + "/confirm-booking.html#details=" + job.ConfirmationToken;
            string deleteLink = site.URL + "/confirm-booking.html#cancel=" + job.ConfirmationToken;

            string time = job.PickupTime.ToShortTimeString() + " (" + job.PickupTime.ToShortDateString() + ")";
            string viaStops = "";
            if (!job.Stops.IsEmpty())
            {
                var stops = job.Stops.Replace(",", "=====").Replace("-----", ",");
                viaStops = stops.Split(',').Length + " via stops: " + stops;
            }

            string returnText = "";
            if (job.JourneyType == "Return")
            {
                returnText = "<br /><br />Return time: <strong> "+job.ReturnTime.ToShortTimeString() + " </strong> ( "+ job.ReturnTime.Month + "/" + job.ReturnTime.Day + "/" + job.ReturnTime.Year +" )";
            }

            var body = GetEmailMessageForBooking(site.Name, site.URL, job.PickFrom, job.DropTo, viaStops, time, job.Fare, confirmationLink, detailsLink, deleteLink, site.Logo, airportCharges, returnText);
            var subject = job.Name + ": Confirm your booking for " + site.URL + "!";

            new Live(site.BookingEmail, site).SendEmail(subject, body, job.Email);
        }

        public static void SendEmail(this Client client, Site site)
        {
            string link = site.URL + "/confirm-account.html#" + client.ConfirmationToken;

            string body = GetEmailMessageForAccount(site.Name, site.URL, site.Description, link, client.Name, site.Logo);
            string subject = client.Name + ": Confirm your account for " + site.URL + "!";

            new Live(site.AccountEmail, site).SendEmail(subject, body, client.Email);
        }

        static string GetEmailMessageForBooking(string siteName, string siteUrl, string pickFrom, string dropTo, string viaStops, string dateTime, string totalFare, string confirmationLink, string detailsLink, string deleteLink, string siteLogo, string airportCharges = null, string returnText = null)
        {
            var airportChargesAndViaStops = "";
            if (!viaStops.IsEmpty())
            {
                airportChargesAndViaStops += "<p style=\"font-size:14px;line-height:1.4em;color:#444444;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;border-top: 1px solid #444;  padding-top: 10px;padding-left: 20px;\">" + viaStops + "</p>";
            }
            if (!airportCharges.IsEmpty())
            {
                airportChargesAndViaStops +=
                    "<p style=\"font-size: 14px;line-height: 1.4em;color: #440606;font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif;border-top: 1px solid #444;padding-top: 10px;padding-left: 20px;background: #FFB0B0;padding-bottom: 15px;margin-bottom:-15px!important;\"><strong>Stansted</strong> airport applies <strong>£2</strong> extra charges and <strong>Luton</strong> applies <strong>£1</strong>.These charges ( <strong>£" + airportCharges + "</strong> ) are included in your final fare.</p>";
            }

            const string message = "<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#DDDDDD\" style=\"background: #dddddd; width: 100%\"> <tbody> <tr> <td> <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"padding: 10px; width: 100%\" width=\"550\"> <tbody> <tr> <td> <div style=\"max-width: 600px; margin: 0 auto\"> <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\" style=\"background-color: #fff; text-align: left; margin: 0 auto; max-width: 1024px; min-width: 320px; width: 100%\"> <tbody> <tr> <td> <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" height=\"8\" background=\"http://s.wordpress.com/i/emails/stripes.gif\" style=\"background-image: url('http:/s.wordpress.com/i/emails/stripes.gif'); background-repeat: repeat-x; background-color: #43a4d0; min-height: 8px; width: 100%\"> <tbody> <tr> <td> </td> </tr> </tbody> </table> <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"background-color: #efefef; padding: 0; border-bottom: 1px solid #ddd; width: 100%\"> <tbody> <tr> <td> <h2 style=\"padding: 0; margin: 5px 20px; font-size: 16px; line-height: 1.4em; font-weight: normal; color: #464646; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif\"> Confirm your booking at <strong>{site-name}</strong>! </h2> </td> <td style=\"vertical-align: middle\" height=\"32\" width=\"32\" valign=\"middle\" align=\"right\"> <img border=\"0\" style=\"margin: 5px 20px 5px 0; vertical-align: middle; vertical-align: middle\" alt=\"\"> </td> </tr> </tbody> </table> <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"20\" bgcolor=\"#ffffff\" style=\"width: 100%\"> <tbody> <tr> <td> <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"width: 100%\"> <tbody> <tr> <td valign=\"top\" style=\"width: 60px; padding-right: 20px;\"> <a href=\"{site-url}\" style=\"text-decoration: underline; color: #2585b2\" target=\"_blank\"> <img border=\"0\" alt=\"{site-name}\" src=\"{site-logo}\" height=\"150\" width=\"215\"></a> </td> <td valign=\"top\" style=\"border-left: 1px solid #444;\"> <p style=\"font-size: 14px; line-height: 1.4em; color: #444; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; padding-left: 20px;\"> Pick from: <strong>{pick-from}</strong> </p> <p style=\"font-size: 14px; color: #444444; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; border-top: 1px solid #444; padding-top: 10px; padding-left: 20px; line-height: 1.4em;\"> Drop to: <strong>{drop-to}</strong> </p> {via-stops} <p style=\"font-size: 14px; color: #444444; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; border-top: 1px solid #444; padding-top: 10px; padding-left: 20px; line-height: 1.4em;\"> Pickup time: <strong>{date-time}</strong> {return-time} </p> <p style=\"font-size: 14px; line-height: 1.4em; color: #444444; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; border-top: 1px solid #444; padding-top: 10px; padding-left: 20px;\"> Total fare: <strong>£{total-fare}</strong> </p> <div style=\"margin: 0 0 20px 0; font-size: 14px; text-align: center;\"> <p style=\"font-size: 14px; line-height: 1.4em; color: #444444; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; margin: 0 0 1em 0\"> <a href=\"{confirm-link}\" style=\"border-radius: 10em; border: 1px solid #11729E; text-decoration: none; color: white; background-color: #2585B2; padding: 5px 15px; font-size: 16px; line-height: 1.4em; font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-weight: normal; margin-left: 0; white-space: nowrap;\" target=\"_blank\"> Confirm </a>  <a href=\"{details-link}\" style=\"border-radius: 10em; border: 1px solid #11729E; text-decoration: none; color: #0E0D0D; background-color: #ECECEC; padding: 5px 15px; font-size: 16px; line-height: 1.4em; font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-weight: normal; margin-left: 0; white-space: nowrap;\" target=\"_blank\"> Details </a>  <a href=\"{delete-link}\" style=\"border-radius: 10em; border: 1px solid #8A120D; text-decoration: none; color: white; background-color: #FF2D05; padding: 5px 15px; font-size: 16px; line-height: 1.4em; font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-weight: normal; margin-left: 0; white-space: nowrap;\" target=\"_blank\"> Cancel </a> </p> </div> </td> </tr> </tbody> </table> </td> </tr> </tbody> </table> <table border=\"0\" cellspacing=\"0\" width=\"100%\" cellpadding=\"20\" bgcolor=\"#efefef\" style=\"background-color: #efefef; text-align: left; border-top: 1px solid #dddddd; width: 100%\"> <tbody> <tr> <td style=\"border-top: 1px solid #f3f3f3; color: #888; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; font-size: 14px; background: #efefef; margin: 0; padding: 10px 20px 20px\"> <p style=\"font-size: 12px; line-height: 1.4em; margin: 0 0 0 0\"> <strong>Trouble clicking?</strong> Copy and paste this URL into your browser: <br> <a href=\"{confirm-link}\" style=\"text-decoration: underline; color: #2585b2\" target=\"_blank\">{confirm-link}</a> </p> </td> </tr> </tbody> </table> </td> </tr> </tbody> </table> <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" height=\"3\" background=\"http://s.wordpress.com/i/emails/stripes.gif\" style=\"background-image: url('http://s.wordpress.com/i/emails/stripes.gif'); background-repeat: repeat-x; background-color: #43a4d0; min-height: 3px; width: 100%\"> <tbody> <tr> <td> </td> </tr> </tbody> </table> </div> </td> </tr> </tbody> </table> <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"padding-bottom: 2em; color: #555555; font-size: 12px; min-height: 18px; text-align: center; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; width: 100%\"> <tbody> <tr> <td align=\"center\"> <a style=\"font-size: 14px; color: #555555!important; text-decoration: none; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; color: #555!important; font-size: 14px; text-decoration: none\" href=\"{site-url}\" target=\"_blank\">Thanks for booking with <img border=\"0\" src=\"{site-logo}\" alt=\"\" style=\"vertical-align: middle\" width=\"120\" height=\"50\"> {site-name}!</a> </td> </tr> </tbody> </table> </td> </tr> </tbody> </table>";
            return message.Replace("{site-name}", siteName)
                .Replace("{site-url}", siteUrl)
                .Replace("{pick-from}", pickFrom)
                .Replace("{drop-to}", dropTo)
                .Replace("{via-stops}", airportChargesAndViaStops)
                .Replace("{date-time}", dateTime)
                .Replace("{return-time}", returnText)
                .Replace("{total-fare}", totalFare)
                .Replace("{confirm-link}", confirmationLink)
                .Replace("{details-link}", detailsLink)
                .Replace("{delete-link}", deleteLink)
                .Replace("{site-logo}", siteLogo);
        }

        public static void SendBookingConfirmedEmail(this Job job, Site site)
        {
            string detailsLink = site.URL + "/confirm-booking.html#details=" + job.ConfirmationToken;
            string deleteLink = site.URL + "/confirm-booking.html#cancel=" + job.ConfirmationToken;

            string time = job.PickupTime.ToShortTimeString() + " (" + job.PickupTime.ToShortDateString() + ")";
            string viaStops = "";
            if (!job.Stops.IsEmpty())
            {
                var stops = job.Stops.Replace(",", "=====").Replace("-----", ",");
                viaStops = stops.Split(',').Length + " via stops: " + stops;
            }

            string returnText = "";
            if (job.JourneyType == "Return")
            {
                returnText = "<br /><br />Return time: <strong> " + job.ReturnTime.ToShortTimeString() + " </strong> ( " + job.ReturnTime.Month + "/" + job.ReturnTime.Day + "/" + job.ReturnTime.Year + " )";
            }

            var body = GetEmailMessageForBookingConfirmed(site.Name, site.URL, job.PickFrom, job.DropTo, viaStops, time, job.Fare, detailsLink, deleteLink, site.Logo, returnText);
            var subject = job.Name + ": Confirm your booking for " + site.URL + "!";

            new Live(site.BookingEmail, site).SendEmail(subject, body, job.Email);
        }

        static string GetEmailMessageForBookingConfirmed(string siteName, string siteUrl, string pickFrom, string dropTo, string viaStops, string dateTime, string totalFare, string detailsLink, string deleteLink, string siteLogo, string returnText = null)
        {
            var airportChargesAndViaStops = "";
            if (!viaStops.IsEmpty())
            {
                airportChargesAndViaStops += "<p style=\"font-size:14px;line-height:1.4em;color:#444444;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;border-top: 1px solid #444;  padding-top: 10px;padding-left: 20px;\">" + viaStops + "</p>";
            }

            const string message = "<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#DDDDDD\" style=\"background: #dddddd; width: 100%\"> <tbody> <tr> <td> <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"padding: 10px; width: 100%\" width=\"550\"> <tbody> <tr> <td> <div style=\"max-width: 600px; margin: 0 auto\"> <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\" style=\"background-color: #fff; text-align: left; margin: 0 auto; max-width: 1024px; min-width: 320px; width: 100%\"> <tbody> <tr> <td> <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" height=\"8\" background=\"http://s.wordpress.com/i/emails/stripes.gif\" style=\"background-image: url('http:/s.wordpress.com/i/emails/stripes.gif'); background-repeat: repeat-x; background-color: #43a4d0; min-height: 8px; width: 100%\"> <tbody> <tr> <td> </td> </tr> </tbody> </table> <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"background-color: #efefef; padding: 0; border-bottom: 1px solid #ddd; width: 100%\"> <tbody> <tr> <td> <h2 style=\"padding: 0; margin: 5px 20px; font-size: 16px; line-height: 1.4em; font-weight: normal; color: #464646; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif\"> Your booking has been <strong style=\"color:green;\">successfully confirmed</strong> at <strong>{site-name}</strong>! </h2> </td> <td style=\"vertical-align: middle\" height=\"32\" width=\"32\" valign=\"middle\" align=\"right\"> <img border=\"0\" style=\"margin: 5px 20px 5px 0; vertical-align: middle; vertical-align: middle\" alt=\"\"> </td> </tr> </tbody> </table> <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"20\" bgcolor=\"#ffffff\" style=\"width: 100%\"> <tbody> <tr> <td> <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"width: 100%\"> <tbody> <tr> <td valign=\"top\" style=\"width: 60px; padding-right: 20px;\"> <a href=\"{site-url}\" style=\"text-decoration: underline; color: #2585b2\" target=\"_blank\"> <img border=\"0\" alt=\"{site-name}\" src=\"{site-logo}\" height=\"150\" width=\"215\"></a> </td> <td valign=\"top\" style=\"border-left: 1px solid #444;\"> <p style=\"font-size: 14px; line-height: 1.4em; color: #444; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; padding-left: 20px;\"> Pick from: <strong>{pick-from}</strong> </p> <p style=\"font-size: 14px; color: #444444; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; border-top: 1px solid #444; padding-top: 10px; padding-left: 20px; line-height: 1.4em;\"> Drop to: <strong>{drop-to}</strong> </p> {via-stops} <p style=\"font-size: 14px; color: #444444; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; border-top: 1px solid #444; padding-top: 10px; padding-left: 20px; line-height: 1.4em;\"> Pickup time: <strong>{date-time}</strong> {return-time} </p> <p style=\"font-size: 14px; line-height: 1.4em; color: #444444; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; border-top: 1px solid #444; padding-top: 10px; padding-left: 20px;\"> Total fare: <strong>£{total-fare}</strong> </p> <div style=\"margin: 0 0 20px 0; font-size: 14px; text-align: center;\"> <p style=\"font-size: 14px; line-height: 1.4em; color: #444444; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; margin: 0 0 1em 0\"> <a href=\"{details-link}\" style=\"border-radius: 10em; border: 1px solid #11729E; text-decoration: none; color: white; background-color: #2585B2; padding: 5px 15px; font-size: 16px; line-height: 1.4em; font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-weight: normal; margin-left: 0; white-space: nowrap;\" target=\"_blank\"> View Booking Details </a>  <a href=\"{delete-link}\" style=\"border-radius: 10em; border: 1px solid #8A120D; text-decoration: none; color: white; background-color: #FF2D05; padding: 5px 15px; font-size: 16px; line-height: 1.4em; font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-weight: normal; margin-left: 0; white-space: nowrap;\" target=\"_blank\"> Cancel </a> </p> </div> </td> </tr> </tbody> </table> </td> </tr> </tbody> </table> <table border=\"0\" cellspacing=\"0\" width=\"100%\" cellpadding=\"20\" bgcolor=\"#efefef\" style=\"background-color: #efefef; text-align: left; border-top: 1px solid #dddddd; width: 100%\"> <tbody> <tr> <td style=\"border-top: 1px solid #4E4A4A; color: #020202; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; font-size: 14px; background: #DDD; margin: 0; padding: 10px 20px 20px\"> <p style=\"font-size: 12px; line-height: 1.4em; margin: 0 0 0 0\"> <strong>Note</strong>: Please note that for airport pickups for cash jobs, the <strong>car park</strong> is not included in the fare showed by our online booking system, this has to be paid to driver once your journey is completed. If you pay with your card then the car par is included in your payment. For <strong>Airport Drop off</strong> to <strong>Luton</strong> and <strong>Stansted</strong>, the diver may ask you to pay <strong>£1</strong> for <strong>Luton</strong> Airport and <strong>£2</strong> for <strong>Stansted</strong> Airport. This is not charges by us, is charges issue by this airport. We apology for any inconvenient. <br /><br /> Your Sincerity, Dracula Cars Ltd, Team </p> </td> </tr> </tbody> </table> </td> </tr> </tbody> </table> <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" height=\"3\" background=\"http://s.wordpress.com/i/emails/stripes.gif\" style=\"background-image: url('http://s.wordpress.com/i/emails/stripes.gif'); background-repeat: repeat-x; background-color: #43a4d0; min-height: 3px; width: 100%\"> <tbody> <tr> <td> </td> </tr> </tbody> </table> </div> </td> </tr> </tbody> </table> <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"padding-bottom: 2em; color: #555555; font-size: 12px; min-height: 18px; text-align: center; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; width: 100%\"> <tbody> <tr> <td align=\"center\"> <a style=\"font-size: 14px; color: #555555!important; text-decoration: none; font-family: &quot; helvetica neue&quot; ,helvetica,arial,sans-serif; color: #555!important; font-size: 14px; text-decoration: none\" href=\"{site-url}\" target=\"_blank\">Thanks for booking with <img border=\"0\" src=\"{site-logo}\" alt=\"\" style=\"vertical-align: middle\" width=\"120\" height=\"50\"> {site-name}!</a> </td> </tr> </tbody> </table> </td> </tr> </tbody> </table>";
            return message.Replace("{site-name}", siteName)
                .Replace("{site-url}", siteUrl)
                .Replace("{pick-from}", pickFrom)
                .Replace("{drop-to}", dropTo)
                .Replace("{via-stops}", airportChargesAndViaStops)
                .Replace("{date-time}", dateTime)
                .Replace("{return-time}", returnText)
                .Replace("{total-fare}", totalFare)
                .Replace("{details-link}", detailsLink)
                .Replace("{delete-link}", deleteLink)
                .Replace("{site-logo}", siteLogo);
        }

        static string GetEmailMessageForAccount(string siteName, string siteUrl, string siteDescription, string confirmationLink, string clientName, string siteLogo)
        {
            const string message = "<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#DDDDDD\" style=\"background:#dddddd;width:100%\"> <tbody><tr> <td> <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"padding:10px;width:100%\" width=\"550\"> <tbody><tr> <td>  <div style=\"max-width:600px;margin:0 auto\"> <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\" style=\"background-color:#fff;text-align:left;margin:0 auto;max-width:1024px;min-width:320px;width:100%\"> <tbody><tr> <td>  <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" height=\"8\" background=\"http://s.wordpress.com/i/emails/stripes.gif\" style=\"background-image:url('http:/s.wordpress.com/i/emails/stripes.gif');background-repeat:repeat-x;background-color:#43a4d0;min-height:8px;width:100%\"> <tbody><tr> <td></td> </tr> </tbody></table>  <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"background-color:#efefef;padding:0;border-bottom:1px solid #ddd;width:100%\"> <tbody><tr> <td> <h2 style=\"padding:0;margin:5px 20px;font-size:16px;line-height:1.4em;font-weight:normal;color:#464646;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif\"> Confirm your account at <strong>{site-name}</strong>!												</h2> </td> <td style=\"vertical-align:middle\" height=\"32\" width=\"32\" valign=\"middle\" align=\"right\"> <img border=\"0\" style=\"margin:5px 20px 5px 0;vertical-align:middle;vertical-align:middle\" alt=\"\"> </td> </tr> </tbody></table>  <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"20\" bgcolor=\"#ffffff\" style=\"width:100%\"> <tbody><tr> <td> <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"width:100%\"> <tbody><tr> <td valign=\"top\" style=\"width:60px;padding-right: 20px;\"> <a href=\"{site-url}\" style=\"text-decoration:underline;color:#2585b2\" target=\"_blank\"><img border=\"0\" alt=\"{site-name}\" src=\"{site-logo}\" height=\"150\" width=\"215\"></a>														</td>  <td valign=\"top\" style=\"border-left: 1px solid #444;\">  <p style=\"font-size:14px;line-height:1.4em;color:#444444;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;padding-top: 10px;padding-left: 20px;\"> Dear <strong>{client-name}</strong>! </p>  <p style=\"font-size:14px;line-height:1.4em;color:#444444;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;border-top: 1px solid #444;  padding-top: 10px;padding-left: 20px;\"> {site-description} </p>  <p style=\"font-size:14px;line-height:1.4em;color:#444444;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;border-top: 1px solid #444;  padding-top: 10px;padding-left: 20px;\"> <strong>Click</strong> below button to confirm your account and enjoy our free offers and bonus services! </p>  <div style=\"margin:0 0 20px 0;font-size:14px;text-align:center;\"> <p style=\"font-size:14px;line-height:1.4em;color:#444444;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;margin:0 0 1em 0\"> <a href=\"{confirm-link}\" style=\"border-radius:10em;border:1px solid #11729e;text-decoration:none;color:#fff;background-color:#2585b2;padding:5px 15px;font-size:16px;line-height:1.4em;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-weight:normal;margin-left:0;white-space:nowrap\" target=\"_blank\"> Confirm your Account </a> </p> </div> </td> </tr> </tbody></table> </td> </tr> </tbody></table>  <table border=\"0\" cellspacing=\"0\" width=\"100%\" cellpadding=\"20\" bgcolor=\"#efefef\" style=\"background-color:#efefef;text-align:left;border-top:1px solid #dddddd;width:100%\"> <tbody><tr> <td style=\"border-top:1px solid #f3f3f3;color:#888;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;font-size:14px;background:#efefef;margin:0;padding:10px 20px 20px\"> <p style=\"font-size:12px;line-height:1.4em;margin:0 0 0 0\"> <strong>Trouble clicking?</strong> Copy and paste this URL into your browser: <br> <a href=\"{confirm-link}\" style=\"text-decoration:underline;color:#2585b2\" target=\"_blank\">{confirm-link}</a> </p> </td> </tr> </tbody></table>  </td> </tr> </tbody></table>  <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" height=\"3\" background=\"http://s.wordpress.com/i/emails/stripes.gif\" style=\"background-image:url('http://s.wordpress.com/i/emails/stripes.gif');background-repeat:repeat-x;background-color:#43a4d0;min-height:3px;width:100%\"> <tbody><tr> <td></td> </tr> </tbody></table>  </div> </td> </tr> </tbody></table>  <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"padding-bottom:2em;color:#555555;font-size:12px;min-height:18px;text-align:center;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;width:100%\"> <tbody> <tr> <td align=\"center\"> <a style=\"font-size:14px;color:#555555!important;text-decoration:none;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;color:#555!important;font-size:14px;text-decoration:none\" href=\"{site-url}\" target=\"_blank\">Thanks for creating account with <img border=\"0\" src=\"{site-logo}\" alt=\"\" style=\"vertical-align:middle\" width=\"120\" height=\"50\"> {site-name}!</a> </td> </tr> </tbody> </table> </td> </tr> </tbody></table>";
            return message.Replace("{site-name}", siteName)
                 .Replace("{site-url}", siteUrl)
                 .Replace("{site-description}", siteDescription)
                 .Replace("{confirm-link}", confirmationLink)
                 .Replace("{client-name}", clientName)
                 .Replace("{site-logo}", siteLogo);
        }

        public static void SendChangePasswordEmail(this Client client, Site site)
        {
            string link = site.URL + "/confirm-password.html#" + client.ForgotPasswordToken;

            string body = GetEmailMessageForForgotPassword(site.Name, site.URL, site.Description, link, client.Name, site.Logo);
            string subject = client.Name + ": Change your password for " + site.URL + "!";

            new Live(site.AccountEmail, site).SendEmail(subject, body, client.Email);
        }

        static string GetEmailMessageForForgotPassword(string siteName, string siteUrl, string siteDescription, string confirmationLink, string clientName, string siteLogo)
        {
            const string message = "<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#DDDDDD\" style=\"background:#dddddd;width:100%\"> <tbody><tr> <td> <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"padding:10px;width:100%\" width=\"550\"> <tbody><tr> <td>  <div style=\"max-width:600px;margin:0 auto\"> <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\" style=\"background-color:#fff;text-align:left;margin:0 auto;max-width:1024px;min-width:320px;width:100%\"> <tbody><tr> <td>  <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" height=\"8\" background=\"http://s.wordpress.com/i/emails/stripes.gif\" style=\"background-image:url('http:/s.wordpress.com/i/emails/stripes.gif');background-repeat:repeat-x;background-color:#43a4d0;min-height:8px;width:100%\"> <tbody><tr> <td></td> </tr> </tbody></table>  <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"background-color:#efefef;padding:0;border-bottom:1px solid #ddd;width:100%\"> <tbody><tr> <td> <h2 style=\"padding:0;margin:5px 20px;font-size:16px;line-height:1.4em;font-weight:normal;color:#464646;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif\"> Confirm your account at <strong>{site-name}</strong>!												</h2> </td> <td style=\"vertical-align:middle\" height=\"32\" width=\"32\" valign=\"middle\" align=\"right\"> <img border=\"0\" style=\"margin:5px 20px 5px 0;vertical-align:middle;vertical-align:middle\" alt=\"\"> </td> </tr> </tbody></table>  <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"20\" bgcolor=\"#ffffff\" style=\"width:100%\"> <tbody><tr> <td> <table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"width:100%\"> <tbody><tr> <td valign=\"top\" style=\"width:60px;padding-right: 20px;\"> <a href=\"{site-url}\" style=\"text-decoration:underline;color:#2585b2\" target=\"_blank\"><img border=\"0\" alt=\"{site-name}\" src=\"{site-logo}\" height=\"150\" width=\"215\"></a>														</td>  <td valign=\"top\" style=\"border-left: 1px solid #444;\">  <p style=\"font-size:14px;line-height:1.4em;color:#444444;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;padding-top: 10px;padding-left: 20px;\"> Dear <strong>{client-name}</strong>! </p>  <p style=\"font-size:14px;line-height:1.4em;color:#444444;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;border-top: 1px solid #444;  padding-top: 10px;padding-left: 20px;\"> {site-description} </p>  <p style=\"font-size:14px;line-height:1.4em;color:#444444;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;border-top: 1px solid #444;  padding-top: 10px;padding-left: 20px;\"> <strong>Click</strong> below button to change your password. </p>  <div style=\"margin:0 0 20px 0;font-size:14px;text-align:center;\"> <p style=\"font-size:14px;line-height:1.4em;color:#444444;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;margin:0 0 1em 0\"> <a href=\"{confirm-link}\" style=\"border-radius:10em;border:1px solid #11729e;text-decoration:none;color:#fff;background-color:#2585b2;padding:5px 15px;font-size:16px;line-height:1.4em;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;font-weight:normal;margin-left:0;white-space:nowrap\" target=\"_blank\"> Change Password </a> </p> </div> </td> </tr> </tbody></table> </td> </tr> </tbody></table>  <table border=\"0\" cellspacing=\"0\" width=\"100%\" cellpadding=\"20\" bgcolor=\"#efefef\" style=\"background-color:#efefef;text-align:left;border-top:1px solid #dddddd;width:100%\"> <tbody><tr> <td style=\"border-top:1px solid #f3f3f3;color:#888;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;font-size:14px;background:#efefef;margin:0;padding:10px 20px 20px\"> <p style=\"font-size:12px;line-height:1.4em;margin:0 0 0 0\"> <strong>Trouble clicking?</strong> Copy and paste this URL into your browser: <br> <a href=\"{confirm-link}\" style=\"text-decoration:underline;color:#2585b2\" target=\"_blank\">{confirm-link}</a> </p> </td> </tr> </tbody></table>  </td> </tr> </tbody></table>  <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" height=\"3\" background=\"http://s.wordpress.com/i/emails/stripes.gif\" style=\"background-image:url('http://s.wordpress.com/i/emails/stripes.gif');background-repeat:repeat-x;background-color:#43a4d0;min-height:3px;width:100%\"> <tbody><tr> <td></td> </tr> </tbody></table>  </div> </td> </tr> </tbody></table>  <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\" style=\"padding-bottom:2em;color:#555555;font-size:12px;min-height:18px;text-align:center;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;width:100%\"> <tbody> <tr> <td align=\"center\"> <a style=\"font-size:14px;color:#555555!important;text-decoration:none;font-family:&quot;Helvetica Neue&quot;,Helvetica,Arial,sans-serif;color:#555!important;font-size:14px;text-decoration:none\" href=\"{site-url}\" target=\"_blank\">Thanks for creating account with <img border=\"0\" src=\"{site-logo}\" alt=\"\" style=\"vertical-align:middle\" width=\"120\" height=\"50\"> {site-name}!</a> </td> </tr> </tbody> </table> </td> </tr> </tbody></table>";
            return message.Replace("{site-name}", siteName)
                 .Replace("{site-url}", siteUrl)
                 .Replace("{site-description}", siteDescription)
                 .Replace("{confirm-link}", confirmationLink)
                 .Replace("{client-name}", clientName)
                 .Replace("{site-logo}", siteLogo);
        }
    }
}