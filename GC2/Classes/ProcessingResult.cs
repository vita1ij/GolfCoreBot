using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace GC2
{
    public class ProcessingResult
    {
        public int? MessageId { get; set; }
        public String? Text { get; set; }
        public long ChatId { get; set; }
        public IReplyMarkup? Markup { get; set; }
        public bool DisableWebPagePreview { get; set; } = false;
        public bool Delete { get; set; } = false;
        public bool IsHtml { get; set; } = false;
        public int? ReplyTo { get; set; }
        //public Image<Rgba32>? Image { get; set; }
        public object? Image { get; set; }
        public bool ReplaceOriginal { get; set; } = false;
        private List<ProcessingResult>? editMessages;
        public List<ProcessingResult>? EditMessages
        {
            get
            {
                return editMessages ?? new List<ProcessingResult>();
            }
            set
            {
                editMessages = value ?? new List<ProcessingResult>();
            }
        }

        public static ProcessingResult? CreateText(ReceivedMessage message, string? response)
        {
            if (String.IsNullOrEmpty(response)) return null;
            return new ProcessingResult()
            {
                Text = response,
                ChatId = message.ChatId,
                Markup = new ReplyKeyboardRemove(),
                DisableWebPagePreview = true,
                IsHtml = false
            };
        }

        public static ProcessingResult CreateCoordinates(ReceivedMessage message, Coordinates response)
        {
            return new ProcessingResult()
            {
                Text = response.ToString(),
                ChatId = message.ChatId,
                Markup = Constants.Keyboards.CoordinatesKeyboard(response),
                DisableWebPagePreview = true,
                IsHtml = true
            };
        }

        public static ProcessingResult CreateHtml(ReceivedMessage message, string response)
        {
            return new ProcessingResult()
            {
                Text = response,
                ChatId = message.ChatId,
                Markup = new ReplyKeyboardRemove(),
                DisableWebPagePreview = true,
                IsHtml = true
            };
        }
        public static ProcessingResult CreateReply(ReceivedMessage message, string response, bool html = false)
        {
            return new ProcessingResult()
            {
                Text = response,
                ChatId = message.ChatId,
                ReplyTo = message.Id,
                IsHtml = html
            };
        }

        public async Task Finish(TelegramBotClient Bot, ReceivedMessage? receivedMessage = null)
        {
            var result = this;
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
                    if (receivedMessage != null && receivedMessage.Id == result.MessageId && receivedMessage.ReplyToBot && receivedMessage.ReplyTo.HasValue)
                    {
                        await Bot.DeleteMessageAsync(result.ChatId, receivedMessage.ReplyTo.Value);
                    }
                    await Bot.DeleteMessageAsync(result.ChatId, result.MessageId.Value);
                }
                catch (Exception ex)
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
                            result.MessageId ?? (receivedMessage == null ? 0 : receivedMessage.Id),
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
