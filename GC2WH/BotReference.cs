﻿using Telegram.Bot;
using Telegram.Bot.Types;

namespace GC2WH
{
    public class BotReference
    {
        public static TelegramBotClient Bot;
        public static User BotUser;
        public static long? LastMessageId;
    }
}
