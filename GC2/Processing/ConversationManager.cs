using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GC2
{
    public static class ConversationManager
    {
        public static ProcessingResult? Process(ReceivedMessage message)
        {
            if (!message.ReplyToBot || String.IsNullOrEmpty(message.ReplyToText)) return null;
            var match = Regex.Match(message.ReplyToText, @"\[([A-Za-z])*\]");
            if (match.Success)
            {
                var key = match.Value[1..^1];
                match = Regex.Match(message.ReplyToText, @"{([A-Za-z0-9])*}");
                var parameters = new List<string>();
                while (match.Success)
                {
                    parameters.Add(match.Value[1..^1]);
                    match = match.NextMatch();
                }
                switch (key)
                {
                    case Constants.ConversationKeywords.Login:
                        message.Text = $"/{Constants.Commands.SetAuth} {message.Text}";
                        message.ReplyToText = String.Empty;
                        message.Normalise();
                        return Processing.ProcessMessage(message);
                    case Constants.ConversationKeywords.Password:
                        if (parameters == null || !parameters.Any()) return null;
                        message.Text = $"/{Constants.Commands.SetAuth} {parameters[0]} {message.Text}";
                        message.ReplyToText = String.Empty;
                        message.Normalise();
                        return Processing.ProcessMessage(message);
                    case Constants.ConversationKeywords.Prefix:
                        if (parameters == null || !parameters.Any()) return null;
                        message.Text = $"/{Constants.Commands.SetPrefix} {parameters[0]} {message.Text}";
                        message.ReplyToText = String.Empty;
                        message.Normalise();
                        return Processing.ProcessMessage(message);
                    case Constants.ConversationKeywords.Radius:
                        if (parameters == null || !parameters.Any()) return null;
                        message.Text = $"/{Constants.Commands.SetRadius} {parameters[0]} {message.Text}";
                        message.ReplyToText = String.Empty;
                        message.Normalise();
                        return Processing.ProcessMessage(message);
                }

            }
            return null;
        }
    }
}
