using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types;

namespace GC2
{
    public class ReceivedMessage
    {
        public int? ReplyTo;
        public string? ReplyToText;
        public bool ReplyToBot;

        public string? Text;
        public object? Image;
        public Coordinates? Coordinates;

        public long ChatId;
        public int Id;
        //public long SenderUsername;
        public long SenderId;

        public bool IsCallback;
        public bool PrivateChat;

        ///////////////////
        ///Calculated
        
        public string? Command { get; set; }
        public string? Parameter { get; set; }
        public List<string>? Parameters { get; set; }
        

        public void Normalise()
        {
            Text = Text?.Trim();

            if (Text != null && Text.StartsWith("@"))
            {
                if (Text.Substring(1).StartsWith(Constants.SpecialCommands.BotName))
                {
                    Text = Text.Substring(Constants.SpecialCommands.BotName.Length + 1).Trim();
                }
            }

            if (Text != null 
                && Text.StartsWith("/"))
            {
                var parts = Text
                            .Substring(1)
                            .Split(' ',StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                Command = parts
                            .First()
                            .ToLower();
                if (parts.Count > 1)
                {
                    Parameter = Text.Substring(Command.Length+1).Trim();
                    Parameters = parts
                                    .Skip(1)
                                    .ToList();
                }
            }
        }

    }
}
