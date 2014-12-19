using System.Linq;
using System.Globalization;
using System.Web.WebPages;
using AlinTuriCab.Models;
using System;

namespace AlinTuriCab.Helpers
{
    public static class CommonExtensions
    {
        public static string GetValidatedString(this string text)
        {
            return text.Replace("-equal", "=").Replace("_plus_", "+").Replace("--", " ").Replace("-qmark", "?").Replace("-nsign", "#").Replace("-n", " <br />").Replace("-lt", "&lt;").Replace("-gt", "&gt;").Replace("-amp", "&").Replace("__", "-");
        }

        public static int ToInt(this string text)
        {
            int temp;
            int.TryParse(text, out temp);
            return temp;
        }

        public static double ToDouble(this string text)
        {
            double temp;
            double.TryParse(text, out temp);
            return temp;
        }

        public static string ToText(this int numbers)
        {
            return numbers.ToString(CultureInfo.InvariantCulture).ToText();
        }

        public static string ToText(this string numbers)
        {
            return numbers
                .Replace("1", "xG")
                .Replace("2", "kaH")
                .Replace("3", "Qb")
                .Replace("4", "ldC")
                .Replace("5", "Wew")
                .Replace("6", "Lpv")
                .Replace("7", "scD")
                .Replace("8", "rBq")
                .Replace("9", "htA")
                .Replace("0", "yXg");
        }
        public static string ToNumbers(this string text)
        {
            return text
                .Replace("xG", "1")
                .Replace("kaH", "2")
                .Replace("Qb", "3")
                .Replace("ldC", "4")
                .Replace("Wew", "5")
                .Replace("Lpv", "6")
                .Replace("scD", "7")
                .Replace("rBq", "8")
                .Replace("htA", "9")
                .Replace("yXg", "0");
        }

        public static string Truncate(this string text, int maxLength, bool insertDots = true)
        {
            if (string.IsNullOrEmpty(text) || maxLength <= 0)
                return string.Empty;
            return text.Length > maxLength ? text.Substring(0, maxLength) + (insertDots ? ".." : "") : text;
        }

        public static Vehicle GetVehicle(this string token, CabDataContext db = null)
        {
            if(db == null) db = new CabDataContext();
            var vehicle = db.Vehicles.FirstOrDefault(v => v.Token == token);
            if (vehicle != null) return vehicle;

            return new Vehicle
            {
                Name = "Vehicle not found!",
                Token = "-------"
            };
        }

        public static string GetOptions(this CabDataContext db, string target, string token)
        {
            var result = "";
            var vehicle = db.Vehicles.FirstOrDefault(v => v.Token == token);
            if (vehicle == null) return result;

            int max = 0, i = 0;

            if (target == "number-of-passengers") max = vehicle.MaxPassengers;
            if (target == "luggages") { max = vehicle.MaxLuggages - 1; i = -1; }
            if (target == "hand-lag") { max = vehicle.MaxHandLag - 1; i = -1; }

            if (target == "baby-seats") { max = vehicle.MaxBabySeats - 1; i = -1;}
            if (target == "child-seats") { max = vehicle.MaxChildSeats - 1;i = -1;}

            if (target != "baby-seats" && target != "child-seats") result = "<option>---</option>";

            for (i += 1; i <= max; i++)
            {
                result += string.Format("<option>{0}</option>", i);
            }

            if (result.IsEmpty() || result == "<option>---</option>") result += "<option>0</option>";

            return result;
        }

        public static string GetFormatedDate(this DateTime date)
        {
            return date.Year + "-" + date.Month + "-" + date.Day;
        }

        public static int GetIndex(this string[] array, string term)
        {
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == term) return i;
            }
            return 0;
        }
    }
}