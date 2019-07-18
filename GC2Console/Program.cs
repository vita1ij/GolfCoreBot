using GC2;
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
        public static Telegram.Bot.Types.User BotUser;
        
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

            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static void BotOnCallBackReceived(object sender, CallbackQueryEventArgs e)
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

            ProcessResult(result, receivedMessage);
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

            ProcessResult(result, receivedMessage);
        }

        private static async void ProcessResult(ProcessingResult? result, ReceivedMessage receivedMessage)
        {
            if (result == null) return;

            //edit old messages
            if (result.EditMessages != null)
            {
                foreach (var editResult in result.EditMessages)
                {
                    if (editResult.Text != null && editResult.MessageId.HasValue)
                    {
                        await Bot.EditMessageTextAsync(
                            editResult.ChatId,
                            editResult.MessageId.Value,
                            editResult.Text,
                            editResult.IsHtml ? ParseMode.Html : ParseMode.Default
                            );
                    }
                    if (editResult.Markup != null && editResult.Markup is InlineKeyboardMarkup && editResult.MessageId.HasValue)
                    {
                        await Bot.EditMessageReplyMarkupAsync(
                            editResult.ChatId,
                            editResult.MessageId.Value,
                            editResult.Markup as InlineKeyboardMarkup
                            );
                    }
                }
            }

            //send reply
            //if (result.Image != null)
            //{
            //
            //}
            //else 
            if (result.Delete && result.MessageId.HasValue && !result.ReplaceOriginal)
            {
                try
                {
                    if (receivedMessage.Id == result.MessageId && receivedMessage.ReplyToBot && receivedMessage.ReplyTo.HasValue)
                    {
                        await Bot.DeleteMessageAsync(result.ChatId, receivedMessage.ReplyTo.Value);
                    }
                    await Bot.DeleteMessageAsync(result.ChatId, result.MessageId.Value);
                }
                catch(Exception ex)
                {
                    Log.New(ex);
                }
            }
            if (result.Text != null)
            {
                if (result.ReplaceOriginal)
                {
                    if (result.Text != null && result.MessageId.HasValue)
                    {
                        await Bot.EditMessageTextAsync(
                            result.ChatId,
                            result.MessageId.Value,
                            result.Text,
                            result.IsHtml ? ParseMode.Html : ParseMode.Default
                            );
                    }
                    if (result.Markup != null && result.Markup is InlineKeyboardMarkup)
                    {
                        await Bot.EditMessageReplyMarkupAsync(
                            result.ChatId,
                            result.MessageId ?? receivedMessage.Id,
                            result.Markup as InlineKeyboardMarkup
                            );
                    }
                }
                else
                {
                    await Bot.SendTextMessageAsync(
                        result.ChatId,
                        result.Text,
                        result.IsHtml ? ParseMode.Html : ParseMode.Default,
                        replyMarkup: result.Markup,
                        disableWebPagePreview: result.DisableWebPagePreview,
                        replyToMessageId: result.ReplyTo ?? 0
                        );
                }
            }
        }
    }
}
