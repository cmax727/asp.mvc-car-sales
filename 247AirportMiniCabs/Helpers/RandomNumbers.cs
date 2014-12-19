using System;
using System.Globalization;
using System.Linq;
using AlinTuriCab.Models;

namespace AlinTuriCab.Helpers
{
    public static class RandomNumbers
    {
        internal static string GetRandomNumbers(int length = 6)
        {
            var values = new byte[length];
            var rnd = new Random();
            rnd.NextBytes(values);
            return values.Aggregate(string.Empty, (current, v) => current + v.ToString(CultureInfo.InvariantCulture));
        }

        public static string GenerateCoupen()
        {
            return MakeCoupen() + "-" + MakeCoupen() + "-" + MakeCoupen() + "-" + MakeCoupen();

        }
        static string MakeCoupen()
        {
            return GetRandom() + GetRandom() + GetRandom() + GetRandom();
        }
        static int prevRandomNumber = 0;
        static string GetRandom()
        {
            string list = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z 1 2 3 4 5 6 7 8 9 0";

            back:
            int randomNumber = new Random().Next(1, 36);
            if (randomNumber == prevRandomNumber) goto back;

            prevRandomNumber = randomNumber;
            return list.Split(' ')[randomNumber];
        }
    }
}