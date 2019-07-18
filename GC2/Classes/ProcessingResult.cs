using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
