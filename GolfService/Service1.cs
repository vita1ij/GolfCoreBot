﻿using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Configuration;
using System.IO;
using GolfCore.Daemons;
using System.Threading.Tasks;
using GolfCore.Processing;
using SixLabors.ImageSharp;
using GolfCore.Helpers;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using GolfCore;
using GolfCoreDB.Managers;
using System.ServiceProcess;

namespace GolfService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

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

        public static void Main()
        {
            try
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
                    var me = Bot.GetMeAsync().Result;
                    Console.Title = me.Username;
                    Bot.StartReceiving();
                    Console.WriteLine($"Start listening for @{me.Username}");
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Cannot set up Bot: {0}---------------{1}", ex.Message, ex.InnerException));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var tasksDaemon = DaemonManager.Create(TaskDaemon.Function, Bot);
            var d = tasksDaemon.Subscribe();

            Console.ReadLine();
            Bot.StopReceiving();
            d.Dispose();
        }

        private static async Task ProcessResult(ProcessingResult result)
        {
            //if chat.Id == -1 ==> send private to person.
            if (result == null) return;

            if (result.Image != null)
            {
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        result.Image.SaveAsJpeg(memoryStream);
                        var arr = memoryStream.ToArray();
                        var imageFile = new Telegram.Bot.Types.InputFiles.InputOnlineFile(new MemoryStream(arr));
                        await Bot.SendPhotoAsync(
                               result.ChatId,
                               imageFile
                               );
                    }
                }
                catch (Exception ex)
                {
                    Log.New(ex);
                }
            }
            await Bot.SendTextMessageAsync(
                result.ChatId,
                result.Text,
                result.IsHtml ? ParseMode.Html : ParseMode.Default,
                replyMarkup: result.Markup,
                disableWebPagePreview: result.DisableWebPagePreview,
                replyToMessageId: result.ReplyTo ?? 0
                );
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null) return;

            if (message.Type == MessageType.Text)
            {
                if (message.Text.EndsWith("@SchrodingersGolfBot"))
                {
                    message.Text = message.Text.Substring(0, message.Text.Length - "@SchrodingersGolfBot".Length);
                }

                ProcessingResult? result;

                string? replyToText;
                if (messageEventArgs.Message.ReplyToMessage != null
                    && messageEventArgs.Message.ReplyToMessage.From.Username == "SchrodingersGolfBot"
                    && messageEventArgs.Message.ReplyToMessage.From.IsBot)
                {
                    replyToText = messageEventArgs.Message.ReplyToMessage.Text;
                    result = ConversationsProcessing.Process(replyToText, message.Text, message.Chat.Id);
                }
                else
                {
                    result = MessageProcessing.Process(message.Text, message.Chat.Id, true);
                    //, message.Chat.Type == ChatType.Private
                }

                if (result?.Text == Constants.Text.Conversation.PasswordSuccessResponse)
                {
                    result = GameCommandProcessing.GameStatus(message.Chat.Id);
                }
                if (result == null) return;
                if (result.ChatId == -1)
                {
                    result.ChatId = messageEventArgs.Message.From.Id;
                }

                await ProcessResult(result);
            }
        }

        private static async void BotOnCallBackReceived(object sender, CallbackQueryEventArgs e)
        {
            ProcessingResult? result = CallBackProcessing.Process(
                                                            e.CallbackQuery.Data,
                                                            e.CallbackQuery.Message.Chat.Id,
                                                            e.CallbackQuery.Message.Chat.Type == ChatType.Private ? true : false,
                                                            e.CallbackQuery.From.Id,
                                                            out ProcessingResult? edit);

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
            if (result == null) return;
            if (result.ChatId == -1)
            {
                result.ChatId = e.CallbackQuery.From.Id;
            }
            if (result.Image != null)
            {
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        result.Image.SaveAsJpeg(memoryStream);
                        var arr = memoryStream.ToArray();
                        var imageFile = new Telegram.Bot.Types.InputFiles.InputOnlineFile(new MemoryStream(arr));
                        await Bot.SendPhotoAsync(
                               e.CallbackQuery.Message.Chat.Id,
                               imageFile
                               );
                    }
                }
                catch (Exception ex)
                {
                    Log.New(ex);
                }
            }

            if (result == null || result.Text == null) return;
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