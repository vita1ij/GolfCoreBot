using System;
using System.Collections.Generic;
using System.Text;

namespace GC2.Constants
{
    public static class Replies
    {
        public const string HELP = "This is Encounter Games help bot. (v.2.3-alpha)\r\n"
            + "Available Commands:\r\n"
            //+ "/settings - setting up bot for current chat\r\n"
            + "\r\n"
            + "/game - setting up encounter game\r\n"
            //+ "/setauth - set login//pass for current game\r\n"
            + "/gettask - force get task\r\n"
            //+ "/alltasks - get all previous tasks for current game\r\n"
            //+ "/stat - get statistics for current game, if supported\r\n"
            + "/exitgame - force exit from current game\r\n"
            + "\r\n"
            + "/list (/l) - show your list\r\n"
            + "/listsorted (/ls) - show your list sorted by values\r\n"
            + "/clearlist (/cl) - clear your list\r\n"
            + "#value - adds value to your list \r\n"
            + "\r\n"
            + "/c code - enter code\r\n"
            + ".code - enter code\r\n"
            + "\r\n"
            + "/a - converting coordinates to address and vice versa\r\n"
            + "\r\n"
            + "If message is coordinates in format xx.xxx yy.yyy, bot will reply with links to google / waze (others separators can be used)\r\n"
            //+ "/mylocations - your saved locations\r\n"
            //+ "/checklocation [xx.x yy.y] (/z) - find nearest known location\r\n"
            //+ "\r\n"
            //+ "/hidekeyboard - force hide keyboard\r\n"
            + "\r\n";

        public const string NO_ACTIVE_GAME_CREATE = "No active game. Create?";

        public static string LOCATION_WITH_NAME_FORMAT = "{2}\r\n<a href =\"https://www.waze.com/ul?ll={0},{1}&navigate=yes\">Waze</a>, <a href =\"https://www.google.com/maps/search/?api=1&query={0},{1}\"> Google</a>  ({0},{1})\r\n";

        /// <summary>
        /// {0} Game Id/GUID
        /// {1} Game type
        /// </summary>
        public const string GAME_NO_AUTH = "In game {0}. {1}";
        public const string GAME_FULL = "In active game.";

        public static string AUTH_GET_LOGIN = $"Reply with [{ConversationKeywords.Login}] for the game {{0}}";
        public const string AUTH_GET_LOGIN_FROM_PUBLIC = "Don't write auth info in public chats. Let's do it again here. Now press this link: /setauth";
        public static string AUTH_GET_PASSWORD = $"Reply with [{ConversationKeywords.Password}] for login {{{{{{1}}}}}} in game {{0}}";

        public static string SET_RADIUS_FORMAT = $"Reply with [{ConversationKeywords.Radius}] for the game {{{{{{0}}}}}}";
        public static string SET_PREFIX_FORMAT = $"Reply with [{ConversationKeywords.Prefix}] for the game {{{{{{0}}}}}}";

        public static string EN_CX_NO_DOMAIN = $"Reply with [{ConversationKeywords.Domain}] for the game {{{{{{0}}}}}}";

        public static string MIRROR_REPLY_FORMAT = "Your Mirror Link is:\r\n{0}\r\n\r\nYour Password is\r\n{1}";
    }
}
