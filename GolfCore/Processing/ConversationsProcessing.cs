using GolfCore.GameEngines;
using GolfCoreDB.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace GolfCore.Processing
{
    public static class ConversationsProcessing
    {
        public static ProcessingResult StartConversation(string topic, long chatId)
        {
            return new ProcessingResult(String.Format("So, lets talk about {{{0}}}...",topic), chatId, new ForceReplyMarkup());
        }

        public static ProcessingResult Process(string replyTo, string text, long chatId)
        {
            int start = -1, end = -1;
            string replyToCommandText = null, param = null;

            start = replyTo.IndexOf('{')+1;
            if (start > 0)
            {
                end = replyTo.IndexOf('}', start);
                if (end >= 0)
                {
                    replyToCommandText = replyTo.Substring(start, end - start);
                }
            }
            
            start = replyTo.IndexOf('[')+1;
            if (start > 0)
            {
                end = replyTo.IndexOf(']', start);
                if (end >= 0)
                {
                    param = replyTo.Substring(start, end - start);
                }
            }
            

            switch (replyToCommandText)
            {
                case "foo":
                    return new ProcessingResult("bar", chatId);
                case "login":
                    return new ProcessingResult(String.Format("Ok, now tell me {{password}} for login [{0}].",text), chatId, new ForceReplyMarkup());
                case "password":
                    var game = GameManager.GetActiveGameByChatId(chatId);
                    GameManager.SetAuthToActiveGame(param, text, chatId);
                    var engine = IGameEngine.Get(game, chatId); //update data - needless
                    //check if can connect
                    if (!engine.Login())
                    {
                        //delete wrong auth data
                        GameManager.SetAuthToActiveGame(null, null, chatId);
                        return new ProcessingResult("Wrong username/password", chatId);
                    }
                    return new ProcessingResult("All Set", chatId);
            }

            return null;
        }
    }
}
