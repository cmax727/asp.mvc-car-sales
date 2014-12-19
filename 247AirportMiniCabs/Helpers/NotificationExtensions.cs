using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlinTuriCab.Models;

namespace AlinTuriCab.Helpers
{
    public static class NotificationExtensions
    {
        public static void Send(this Notification notification, CabDataContext db)
        {
            db.Notifications.InsertOnSubmit(notification);
            db.SubmitChanges();
        }
    }
}