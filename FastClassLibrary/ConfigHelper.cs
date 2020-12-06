using System;

namespace FastClassLibrary
{
    public static class ConfigHelper
    {
        public static string PositionList = "2,9,30,18,23,16,12,17,22,1";
        public static string UserName = "4";
        public static string Password = "Fast@2020@DeheusSP";

        public static string DateTimeFormat = "MM/dd/yyyy h:mm tt";

        internal static DateTime GetCurrentDate()
        {
            return DateTime.Now;
        }
    }
}
