/* Muaz Khan – http://muaz-khan.blogspot.com */
using System;

namespace AlinTuriCab.Helpers
{
    public static class ToAgoDateTime
    {
        /// <summary>
        /// Convert date into timeago format
        /// </summary>
        /// <param name="time">date</param>
        /// <param name="diffWith">compare with</param>
        /// <returns>toago formatted string</returns>
        public static string ToAgo(this DateTime time, DateTime? compareWith = null)
        {
            DateTime diffWith = compareWith.HasValue ? compareWith.Value : UKTime.Now;
            if (time > diffWith)
            {
                return time.LaterTime(diffWith);
            }
            return time < diffWith ? time.EarlierTime(diffWith) : time.ToShortDateString();
        }

        #region private methods

        private static string LaterTime(this DateTime time, DateTime differenceWth)
        {
            var timeDifference = time - differenceWth;
            var days = timeDifference.Days;

            var result = string.Empty;

            if (days > 30) return time.ToShortDateString();

            if (days == 1)
            {
                result += days + " day ";
                goto End;
            }
            else if (days > 1)
            {
                result += days + " days ";
                goto End;
            }

            var hours = timeDifference.Hours;

            if (hours == 1)
            {
                result += hours + " hour ";
                goto End;
            }
            else if (hours > 1)
            {
                result += hours + " hours ";
                goto End;
            }

            var minutes = timeDifference.Minutes;
            if (minutes == 1)
            {
                result += minutes + " minute ";
            }
            else if (minutes > 1)
            {
                result += minutes + " minutes ";
            }
        End:
            return string.IsNullOrWhiteSpace(result) ? "a few seconds ago" : result + "later";
        }

        private static string EarlierTime(this DateTime time, DateTime differenceWth)
        {
            var timeDifference = differenceWth - time;
            var days = timeDifference.Days;

            var result = string.Empty;

            if (days > 30) return time.ToShortDateString();

            if (days == 1)
            {
                result += days + " day ";
                goto End;
            }
            else if (days > 1)
            {
                result += days + " days ";
                goto End;
            }

            var hours = timeDifference.Hours;

            if (hours == 1)
            {
                result += hours + " hour ";
                goto End;
            }
            else if (hours > 1)
            {
                result += hours + " hours ";
                goto End;
            }

            var minutes = timeDifference.Minutes;
            if (minutes == 1)
            {
                result += minutes + " minute ";
            }
            else if (minutes > 1)
            {
                result += minutes + " minutes ";
            }
        End:
            return
                string.IsNullOrWhiteSpace(result)
                ? "a few seconds ago"
                : result
                + "ago";
        }

        #endregion
    }
}