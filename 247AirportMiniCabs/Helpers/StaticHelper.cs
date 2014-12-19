using System.Linq;
using AlinTuriCab.Models;
using System.Collections.Generic;

namespace AlinTuriCab.Helpers
{
    public static class StaticHelper
    {
        public static PostCodesDataContext _db;

        public static PostCodesDataContext PostCodes
        {
            get { return _db ?? (_db = new PostCodesDataContext {ObjectTrackingEnabled = false}); }
        }

        public static bool IsOnline
        {
            get { return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable(); }
        }

        public static Site Site(this CabDataContext db)
        {
            return db.Sites.FirstOrDefault(s => s.Token == "airportsminicabs247");
        }

        private static List<AlphabetCodes> _Alphabet;

        public static List<AlphabetCodes> Alphabets
        {
            get
            {
                if (_Alphabet == null)
                {
                    _Alphabet = new List<AlphabetCodes>();
                    _Alphabet.Add(new AlphabetCodes
                                      {
                                          PostCode = "a",
                                          Take = 1,
                                          Skip = 24366
                                      });
                }
                return _Alphabet;
            }
        }

        public static string JobURI
        {
            get { return "joburi@draculacars.co.uk"; }
        }
    }

    public class AlphabetCodes
    {
        public string PostCode { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
    }
}