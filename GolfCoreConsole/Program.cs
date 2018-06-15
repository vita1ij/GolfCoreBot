using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Configuration;
using System.IO;
using GolfCore.Daemons;
using System.Threading.Tasks;

namespace GolfCoreConsole
{
    class Program
    {
        public static IConfiguration Config
        {
            get
            {
                var c = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("config.json");
                return c.Build();
            }
        }

        public static readonly TelegramBotClient Bot = new TelegramBotClient(Config["API_KEY"].ToString());

        public static void Main(string[] args)
        {
            //Daemons
            TaskDaemon taskDaemon = new TaskDaemon();

            Bot.OnMessage += BotOnMessageReceived;
            //Bot.OnMessageEdited += BotOnMessageReceived;

            var me = Bot.GetMeAsync().Result;

            Console.Title = me.Username;

            Bot.StartReceiving();
            Console.WriteLine($"Start listening for @{me.Username}");

            while (!Console.KeyAvailable) {
                if (Boolean.Parse(Config["Minute_Daemons"].ToString()))
                {
                    taskDaemon.RunAsync(Bot).Wait();
                }
            }

            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null) return;

            if (message.Type == MessageType.Text)
            {

                IReplyMarkup keyboard = new ReplyKeyboardRemove();
                if (message.Text.EndsWith("@SchrodingersGolfBot"))
                {
                    message.Text = message.Text.Substring(0, message.Text.Length - "@SchrodingersGolfBot".Length);
                }
                var result = GolfCore.Processing.MessageProcessing.Process(message.Text, message.Chat.Id);
                if (result == null) return;
                await Bot.SendTextMessageAsync(
                    result.ChatId,
                    result.Text,
                    result.IsHtml ? ParseMode.Html : ParseMode.Default,
                    replyMarkup: result.Markup,
                    disableWebPagePreview: result.DisableWebPagePreview);
            }
        }
    }
}
