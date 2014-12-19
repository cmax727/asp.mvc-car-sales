using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlinTuriCab.Models;
using System.Web.WebPages;

namespace AlinTuriCab.Helpers
{
    public static class DiscountHelper
    {
        static string GenerateDiscount(this CabDataContext db, string percent)
        {
            back:
            string token = RandomNumbers.GenerateCoupen();
            if (db.Discounts.Any(d => d.Token == token)) goto back;
            var discount = new Discount
            {
                ClientToken = LoginHelper.Client.Token,
                GeneratedAt = UKTime.Now,
                ValidTill = UKTime.Now.AddDays(10),
                Percent = percent,
                Token = token,
                IsExpired = false
            };

            db.Discounts.InsertOnSubmit(discount);
            db.SubmitChanges();

            return token;
        }

        public static string GetDiscountToken(this CabDataContext db, string percent)
        {
            var discount = db.Discounts.FirstOrDefault(d => !d.IsExpired && d.ValidTill > UKTime.Now && d.ClientToken == LoginHelper.Client.Token);
            if (discount == null) return db.GenerateDiscount(percent);

            return discount.Token;
        }

        public static string GetDiscountValidTill(this CabDataContext db)
        {
            var discount = db.Discounts.FirstOrDefault(d => !d.IsExpired && d.ValidTill > UKTime.Now && d.ClientToken == LoginHelper.Client.Token);
            return discount != null ? discount.ValidTill.ToLongDateString() : "...";
        }
    }
}