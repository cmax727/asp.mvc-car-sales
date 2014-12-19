using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlinTuriCab.Models;

namespace AlinTuriCab.Helpers
{
    public static class LoginHelper
    {
        public static Client Client { get; set; }

        public static void ValidateClientRequest(this CabDataContext db, string token)
        {
            Client = db.Clients.FirstOrDefault(c => c.Token == token);
        }

        public static void InvalidLoginRequest()
        {
            Client = null;
        }
    }
}