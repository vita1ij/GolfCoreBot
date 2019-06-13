using GolfCore.GameEngines;
using GolfCoreDB.Data;
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

        public static ProcessingResult? Process(string replyTo, string text, long chatId)
        {
            int start,end;
            string? replyToCommandText = null;
            string? param = null;
            Game game;
            IGameEngine engine;

            start = replyTo.IndexOf('{')+1;
            if (start > 0)
            {
                end = replyTo.IndexOf('}', start);
                if (end >= 0)
                {
                    replyToCommandText = replyTo.Substring(start,end-start);
                    //replyToCommandText = replyTo[start..end];
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
            

            switch (replyToCommandText ?? "")
            {
                case Constants.Commands.ConversationComands.Login :
                    return new ProcessingResult(String.Format(Constants.Text.Conversation.LoginResponse,text), chatId, new ForceReplyMarkup());
                case Constants.Commands.ConversationComands.Password:
                    if (param == null) return null;
                    game = GameManager.GetActiveGameByChatId(chatId);
                    GameManager.GetAuthForActiveGame(chatId, out var login, out var pass);
                    GameManager.SetAuthToActiveGame(param, text, chatId);
                    engine = IGameEngine.Get(game, chatId);
                    //check if can connect
                    if (!engine.Login())
                    {
                        //delete wrong auth data
                        GameManager.SetAuthToActiveGame(login, pass, chatId);
                        return new ProcessingResult(Constants.Text.Conversation.PasswordErrorResponse, chatId);
                    }
                    return new ProcessingResult(Constants.Text.Conversation.PasswordSuccessResponse, chatId);
                default:
                    return null;
            }
        }
    }
}
