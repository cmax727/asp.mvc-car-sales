using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlinTuriCab.Helpers
{
    public struct JobType
    {
        public static string JobConfirmed = "Job Confirmed";
        public static string NewJob = "New Job";
        public static string ResentCode = "Resent Confirmation Code";
        public static string SentCodeForJob = "Sent Job Confirmation Code";
        public static string AccountConfirmed = "Account Confirmed";
        public static string JobError = "Job Error";
        public static string DirectMessage = "Direct Message";
    }
}