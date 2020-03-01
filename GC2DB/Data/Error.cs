using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GC2DB.Data
{
    public class Error
    {
        [Key]
        public long Id { get; set; }
        public string Text { get; set; }
        public string? Stack { get; set; }
        public DateTime Date { get; set; }
        public long? Chat { get; set; }
        public long? Sender { get; set; }

        public Error()
        {
        }

        public Error(
                    Exception? ex = null
                    , string text = "Oups"
                    , string? stack = null
                    ,long? chat = null
                    ,long? sender = null
            )
        {
            Sender = sender;
            Chat = chat;
            if (ex != null)
            {
                text = ex.Message;
                stack = ex.StackTrace;
                if (ex.InnerException != null)
                {
                    text += $"^|^{ex.InnerException?.Message ?? String.Empty}";
                    stack += $"\r\n\r\n^|^\r\n\r\n{ex.InnerException?.StackTrace ?? String.Empty}";
                }
            }
            Text = text;
            Stack = stack;
            Date = DateTime.Now;
        }
    }
}
