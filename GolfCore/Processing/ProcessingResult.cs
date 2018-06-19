using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace GolfCore.Processing
{
    public class ProcessingResult
    {
        public String Text { get; set; }
        public long ChatId { get; set; }
        public IReplyMarkup Markup { get; set; }
        public bool DisableWebPagePreview { get; set; }
        public bool IsHtml { get; set; }
        public int? ReplyTo { get; set; }

        public ProcessingResult(string text, long chatId, bool disablePreview = true, bool isHtml = false)
        {
            Text = text;
            ChatId = chatId;
            Markup = new ReplyKeyboardRemove();
            DisableWebPagePreview = disablePreview;
            IsHtml = isHtml;
        }

        public ProcessingResult(string text, long chatId, IReplyMarkup markup, bool disablePreview = true, bool isHtml = false, int? replyTo = null)
        {
            Text = text;
            ChatId = chatId;
            Markup = markup;
            DisableWebPagePreview = disablePreview;
            IsHtml = isHtml;
            ReplyTo = replyTo;
        }
    }
}
