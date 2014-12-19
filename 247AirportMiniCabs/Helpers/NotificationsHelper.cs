using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlinTuriCab.Models;

namespace AlinTuriCab.Helpers
{
    public static class NotificationsHelper
    {
        static void AddNewNotification(this Notification notification, CabDataContext db)
        {
        BackAgain:
            var token = RandomNumbers.GetRandomNumbers(10);
            if (db.Notifications.Any(j => j.Token == token)) goto BackAgain;

            notification.Token = token;
            notification.SentAt = UKTime.Now;
            notification.Status = "New";

            db.Notifications.InsertOnSubmit(notification);
            db.SubmitChanges();
        }
        public static void JobConfirmationNotification(this Job job, CabDataContext db)
        {
            new Notification
            {
                JobToken = job.Token,
                Sender = "Automatic",
                Receiver = job.ClientToken ?? job.Email,       //-------------- for anonymouse clients!
                Type = JobType.JobConfirmed,
                Message = "Congratulation Sir,<br /><br />You've successfully confirmed your job."
            }.AddNewNotification(db);
        }
        public static void JobSentConfirmationCode(this Job job, CabDataContext db)
        {
            new Notification
            {
                Sender = "Automatic",
                Receiver = job.Token,
                Type = JobType.SentCodeForJob,
                Message = "Dear <span class=\"roshan\">" + job.Name + "</span>,<br /><br />We've sent you a confirmation code at your email: <span class=\"roshan\"><a href=\"mailto:" + job.Email + "\">" + job.Email + "</a></span>. Please confirm your job as soon as possible and enjoy our bonus services!"
            }.AddNewNotification(db);
        }

        public static void ClientConfirmationNotification(this Client client, CabDataContext db)
        {
            new Notification
            {
                Sender = "Automatic",
                Receiver = client.Token,
                Type = JobType.AccountConfirmed,
                Message = "Congratulation Sir,<br /><br />You've successfully confirmed your account."
            }.AddNewNotification(db);
        }

        public static void ClientResentConfirmationCode(this Client client, CabDataContext db)
        {
            new Notification
            {
                Sender = "Admin",
                Receiver = client.Token,
                Type = JobType.ResentCode,
                Message = "Dear <span class=\"roshan\">" + client.Name + "</span>,<br /><br />We've sent you a confirmation code again at your email: <span class=\"roshan\"><a href=\"mailto:" + client.Email + "\">" + client.Email + "</a></span>. Please confirm your account as soon as possible and enjoy our bonus services!"
            }.AddNewNotification(db);
        }

        public static void NotifyOperator(this CabDataContext db, string type, string message, string jobToken = null)
        {
            new Notification
            {
                JobToken = jobToken,
                Sender = "Automatic",
                Receiver = "Operator",
                Type = type,
                Message = message
            }.AddNewNotification(db);
        }

        public static void NotifyAdmin(this CabDataContext db, string type, string message, string jobToken = null)
        {
            new Notification
            {
                JobToken = jobToken,
                Sender = "Automatic",
                Receiver = "Admin",
                Type = type,
                Message = message
            }.AddNewNotification(db);
        }

        public static void NotifyClient(this Client client, CabDataContext db, string type, string message, string jobToken = null)
        {
            new Notification
            {
                JobToken = jobToken,
                Sender = "Automatic",
                Receiver = client.Token,
                Type = type,
                Message = message
            }.AddNewNotification(db);
        }
    }
}