using System;
using System.Collections.Generic;
using System.Text;

namespace GolfCore
{
    public class Constants
    {
        public static string CoordinatesString = "<a href =\"https://www.waze.com/location?ll={0},{1}&navigate=yes\">Waze</a>, <a href =\"https://www.google.com/maps/search/?api=1&query={0},{1}\"> Google</a>  ({0},{1})";

        public static class Commands
        {
            public const string ShowSettings = "showsettings";
            public const string UpdateSetting = "updatesetting";
            public const string StartGame = "startgame";
            public const string JoinGame = "joingame";
            public const string EndGame = "endgame";
        }

        public static class Settings
        {
            public const string GameLogin = "GameLogin";
            public const string GamePass = "GamePass";
        }

        public static class Exceptions
        {
            public static string GameLoginInfoNotAvailable { get { return "Set up game Auth info."; } }

            public static string WrongParameterCount(long count) { return String.Format("There should be {0} parameters", count.ToString()); }
        }

        public const string Help = "Commands:\r\n" +
            "/startgame\r\n" +
            "/joingame\r\n" +
            "/exitfromgame\r\n" +
            "/setauth\r\n" +
            "/settaskmonitoring\r\n";
    }
}
