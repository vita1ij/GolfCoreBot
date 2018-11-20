using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Configuration;
using System.IO;
using GolfCore.Daemons;
using System.Threading.Tasks;
using GolfCore.Processing;

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
            Bot.OnCallbackQuery += BotOnCallBackReceived;
            //Bot.OnMessageEdited += BotOnMessageReceived;

            var me = Bot.GetMeAsync().Result;

            Console.Title = me.Username;

            Bot.StartReceiving();
            Console.WriteLine($"Start listening for @{me.Username}");

            while (!Console.KeyAvailable) {
                if (Config["Minute_Daemons"].ToString() == "true")
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
                
                var result = GolfCore.Processing.MessageProcessing.Process(message.Text, message.Chat.Id, message.MessageId);
                if (result == null || result.Text == null) return;
                await Bot.SendTextMessageAsync(
                    result.ChatId,
                    result.Text,
                    result.IsHtml ? ParseMode.Html : ParseMode.Default,
                    replyMarkup: result.Markup,
                    disableWebPagePreview: result.DisableWebPagePreview,
                    replyToMessageId: result.ReplyTo.HasValue ? result.ReplyTo.Value : 0
                    );
            }
        }

        private static async void BotOnCallBackReceived(object sender, CallbackQueryEventArgs e)
        {
            ProcessingResult edit;
            ProcessingResult result = CallBackProcessing.Process(
                                                            e.CallbackQuery.Data, 
                                                            e.CallbackQuery.Message.Chat.Id, 
                                                            e.CallbackQuery.Message.Chat.Type == ChatType.Private ? true : false,
                                                            out edit);
            
            if (edit != null)
            {
                //await Bot.EditMessageReplyMarkupAsync(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Message.MessageId, (InlineKeyboardMarkup)edit.Markup);
                await Bot.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id,
                    e.CallbackQuery.Message.MessageId,
                    edit.Text,
                    edit.IsHtml ? ParseMode.Html : ParseMode.Default,
                    replyMarkup: (InlineKeyboardMarkup)edit.Markup,
                    disableWebPagePreview: edit.DisableWebPagePreview
                    );
            }

            if (result == null || result.Text == null) return;
            await Bot.SendTextMessageAsync(
                result.ChatId,
                result.Text,
                result.IsHtml ? ParseMode.Html : ParseMode.Default,
                replyMarkup: result.Markup,
                disableWebPagePreview: result.DisableWebPagePreview,
                replyToMessageId: result.ReplyTo.HasValue ? result.ReplyTo.Value : 0
                );
        }
    }
}
