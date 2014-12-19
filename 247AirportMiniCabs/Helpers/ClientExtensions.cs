using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlinTuriCab.Models;

namespace AlinTuriCab.Helpers
{
    public static class ClientExtensions
    {
        public static Job GetLatestJob(this Client client, CabDataContext db)
        {
            return db.Jobs.OrderByDescending(o => o.ID).FirstOrDefault(j => j.ClientToken == client.Token);
        }
    }
}