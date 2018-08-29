using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace GolfCore
{
    public class Constants
    {
        public static string CoordinatesString = "<a href =\"https://www.waze.com/location?ll={0},{1}&navigate=yes\">Waze</a>, <a href =\"https://www.google.com/maps/search/?api=1&query={0},{1}\"> Google</a>  ({0},{1})";

        public static class Text
        {
            public const string SetAuth = "To Set auth, write '/setauth login pass'. It is better to use private chat for that.";
        }

        public static class Commands
        {
            public const string ShowSettings = "showsettings";
            public const string UpdateSetting = "updatesetting";
            public const string CheckLocation = "checklocation";
            public const string Help = "help";
            public const string StartGame = "startgame";
            public const string JoinGame = "joingame";
            public const string EndGame = "endgame";
            public const string Game = "game";
            public const string ExitFromGame = "exitfromgame";
            public const string SetAuth = "setauth";
            public const string GetTask = "gettask";
            public const string SetTaskMonitoringStatus = "settaskmonitoring";
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

        public static class Keyboards
        {
            public static InlineKeyboardMarkup NoActiveGame = new InlineKeyboardMarkup(new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "En.cx", CallbackData = "CreateEncx" },
                new InlineKeyboardButton() { Text = "Igra.lv", CallbackData = "CreateIgra" }
            });

            public static InlineKeyboardMarkup NoAuthGame = new InlineKeyboardMarkup(new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Set auth", CallbackData = "SetAuth" },
                new InlineKeyboardButton() { Text = "Join link", CallbackData = "Joinlink" },
                new InlineKeyboardButton() { Text = "Exit game", CallbackData = "ExitGame" },
            });

            //todo: changeto enum
            public static InlineKeyboardMarkup ActiveGame(int taskDaemon, int statisticsDaemon)
            {
                var result = new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>() {
                    new List<InlineKeyboardButton>()
                    {
                        new InlineKeyboardButton() { Text = "Join link", CallbackData = "Joinlink" },
                        new InlineKeyboardButton() { Text = "Exit game", CallbackData = "ExitGame" }
                    },
                    new List<InlineKeyboardButton>()
                    {
                        new InlineKeyboardButton() { Text = "Get Task", CallbackData = "GetTask" },
                        new InlineKeyboardButton() { Text = taskDaemon == 0 ? "[[Disabled]]" :  "Disabled", CallbackData = "DisableTaskUpdates" },
                        new InlineKeyboardButton() { Text = taskDaemon == 1 ? "[[Up only]]" :  "Up only", CallbackData = "EnableTaskUpdates" },
                        new InlineKeyboardButton() { Text = taskDaemon == 2 ? "[[With text]]" :  "With text", CallbackData = "EnableTaskTask" }
                    },
                    new List<InlineKeyboardButton>()
                    {
                        new InlineKeyboardButton() { Text = statisticsDaemon == 0 ? "[[No Statistics]]" : "No Statistics", CallbackData = "DisableStatisticUpdates" },
                        new InlineKeyboardButton() { Text = statisticsDaemon == 1 ? "[[Same lvl]]" : "Same lvl", CallbackData = "EnableStatisticLvl" },
                        new InlineKeyboardButton() { Text = statisticsDaemon == 2 ? "[[All Statistics]]" : "All Statistics", CallbackData = "EnableStatistics" }
                    },
                });
                return result;
            }
        }

        public const string Help = "Commands:\r\n" +
            "/game - game settings\r\n" +
            "\r\n" + 
            "/startgame\r\n" +
            "/joingame\r\n" +
            "/exitfromgame\r\n" +
            "/setauth\r\n" +
            "/settaskmonitoring\r\n";
    }
}
