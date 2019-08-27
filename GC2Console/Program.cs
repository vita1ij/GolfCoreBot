using GC2;
using GC2.Daemons;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace GC2Console
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

        public static TelegramBotClient Bot;
        public static User BotUser;
        
        static void Main(string[] args)
        {
            Setup();
        }

        private static void Setup()
        {
            if (Config == null) throw new Exception("No config");
            if (Config["API_KEY"] == null) throw new Exception("No API key");
            if (Config["ERROR_FOLDER"] == null) throw new Exception("No Error folder selected");
            if (String.IsNullOrEmpty(Config["API_KEY"])) throw new Exception("Empty API key");
            try
            {
                Bot = new TelegramBotClient(Config["API_KEY"].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Cannot initialise Bot: {0}---------------{1}", ex.Message, ex.InnerException));
            }
            try
            {
                Bot.OnMessage += BotOnMessageReceived;
                Bot.OnCallbackQuery += BotOnCallBackReceived;
                BotUser = Bot.GetMeAsync().Result;
                GC2.Constants.SpecialCommands.BotName = BotUser.Username;
                Bot.StartReceiving();
                Console.WriteLine($"Start listening for @{BotUser.Username}");
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Cannot set up Bot: {0}---------------{1}", ex.Message, ex.InnerException));
            }

            try
            {
                new TaskDaemon().Create(Bot).Subscribe();
            }
            catch(Exception ex)
            {
                Log.New(ex);
            }

            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void BotOnCallBackReceived(object sender, CallbackQueryEventArgs e)
        {
            var callback = e.CallbackQuery;
            if (callback == null) return;
            var receivedMessage = new ReceivedMessage()
            {
                Text = callback.Data,
                ChatId = callback.Message.Chat.Id,
                Id = callback.Message.MessageId,
                PrivateChat = (callback.Message.Chat.Type == ChatType.Private),
                IsCallback = true,
                SenderId = callback.From.Id,
            };

            receivedMessage.Normalise();

            var result = Processing.ProcessMessage(receivedMessage);
            if (result == null) return;
            await result.Finish(Bot, receivedMessage);
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            if (message == null) return;

            var receivedMessage = new ReceivedMessage()
            {
                Text = message.Text,
                Image = message.Photo,
                Coordinates = (message.Location == null) ? null : new Coordinates(message.Location),

                ChatId = message.Chat.Id,
                Id = message.MessageId,
                PrivateChat = (message.Chat.Type == ChatType.Private),
                IsCallback = false,
                SenderId = message.From.Id,

                ReplyTo = (message.ReplyToMessage == null) ? (int?)null : message.ReplyToMessage.MessageId,
                ReplyToText = (message.ReplyToMessage == null) ? (string?)null : message.ReplyToMessage.Text,
                ReplyToBot = (message.ReplyToMessage == null) ? false : (message.ReplyToMessage.From.IsBot && message.ReplyToMessage.From.Id == Bot.BotId),
            };

            receivedMessage.Normalise();

            if (message.Photo != null)
            {
                if (message.Photo.Length > 0)
                {
                    var obj = message.Photo.GetValue(message.Photo.Length - 1);
                    if (obj != null && obj is PhotoSize)
                    {
                        PhotoSize? ps = obj as PhotoSize;
                        if (ps != null)
                        {
                            var imageId = ps.FileId;
                            using (var ms = new MemoryStream())
                            {
                                receivedMessage.Image = await Bot.GetInfoAndDownloadFileAsync(imageId, ms);
                                receivedMessage.Image = ms.ToArray();
                            }
                        }
                    }
                }
            }

            var result = Processing.ProcessMessage(receivedMessage);
            if (result == null) return;
            await result.Finish(Bot, receivedMessage);
        }
    }
}
