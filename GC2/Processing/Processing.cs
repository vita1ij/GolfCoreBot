using GC2.Helpers;
using GC2DB.Data;
using GC2DB.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GC2
{
    public class Processing
    {
        public static ProcessingResult? ProcessMessage(ReceivedMessage message)
        {
            try
            {
                string? resultString = null;
                if (!String.IsNullOrWhiteSpace(message.Command))
                {
                    switch (message.Command)
                    {
                        case "u":
                            if (message.Parameter == "happy?")
                                return ProcessingResult.CreateText(message, (new Random(DateTime.Now.Second).Next(0, 2) == 1) ? "Yes" : "No");
                            return null;
                        case "foo":
                            return ProcessingManager.Foo(message);
                        case Constants.Commands.Start:
                        case Constants.Commands.Help:
                        case Constants.Commands.HelpShort:
                            return ProcessingResult.CreateText(message, Constants.Replies.HELP);
                        case Constants.Commands.Address:
                            return ProcessingManager.ProcessAddress(message);
                        case Constants.Commands.List:
                        case Constants.Commands.ListShort:
                            resultString = ListManager.GetList(message.ChatId);
                            if (resultString != null)
                            {
                                return ProcessingResult.CreateText(message, resultString);
                            }
                            return null;
                        case Constants.Commands.ListSorted:
                        case Constants.Commands.ListSortedShort:
                            resultString = ListManager.GetList(message.ChatId, true);
                            if (resultString != null)
                            {
                                return ProcessingResult.CreateText(message, resultString);
                            }
                            return null;
                        case Constants.Commands.ClearList:
                        case Constants.Commands.ClearListShort:
                            ListManager.ClearList(message.ChatId);
                            return null;
                        case Constants.Commands.SaveCoordinates:
                            return ProcessingManager.SaveCoordinates(message);
                        case Constants.Commands.GetChatId:
                            return ProcessingResult.CreateText(message, message.ChatId.ToString());
                        case Constants.Commands.DeleteMessage:
                            return new ProcessingResult() { Delete = true, ChatId = message.ChatId, MessageId = message.Id };
                        case Constants.Commands.GameSetup:
                            return ProcessingManager.GameSetup(message);
                        case Constants.Commands.CreateDemoGame:
                            return ProcessingManager.CreateGame(message, GameType.Demo);
                        case Constants.Commands.CreateQuestGame:
                            return ProcessingManager.CreateGame(message, GameType.Quest);
                        case Constants.Commands.CreateEncxGame:
                            return ProcessingManager.CreateGame(message, GameType.CustomEnCx);
                        case Constants.Commands.CreateIgraGame:
                            return ProcessingManager.CreateGame(message, GameType.IgraLv);
                        case Constants.Commands.SetAuth:
                            return ProcessingManager.StartSetingAuth(message);
                        case Constants.Commands.JoinGameLink:
                            return ProcessingManager.JoinGame(message);
                        case Constants.Commands.ExitGame:
                            return ProcessingManager.ExitFromGame(message);
                        case Constants.Commands.ShowGameSettings:
                            return ProcessingManager.ShowGameSettings(message);
                        case Constants.Commands.GetTask:
                            return ProcessingManager.GetTask(message);
                        case Constants.Commands.FindEncxGame:
                            return ProcessingManager.FindEnCxGame(message);
                        case Constants.Commands.FoundEncxGame:
                            return ProcessingManager.FoundEncxGame(message);
                        case Constants.Commands.SetPrefix:
                            return ProcessingManager.SetPrefix(message);
                        case Constants.Commands.SetRadius:
                            return ProcessingManager.SetRadius(message);
                        case Constants.Commands.GameTaskNoUpdates:
                        case Constants.Commands.GameTaskUpdateStatus:
                        case Constants.Commands.GameTaskUpdateText:
                            return ProcessingManager.SetTaskUpdate(message);
                        case Constants.Commands.EnterCode:
                            return ProcessingManager.EnterCode(message);
                        case Constants.Commands.MirrorLink:
                            return ProcessingManager.MirrorLink(message);
                        default:
                            break;
                    }
                }
                if (message.Text != null && message.Text.StartsWith(Constants.SpecialCommands.AddListValue))
                {
                    ListManager.AddValue(message.ChatId, message.Text.Substring(Constants.SpecialCommands.AddListValue.Length));
                    return null;
                }
                if (message.Text != null && message.Text.StartsWith(Constants.SpecialCommands.EnterCode))
                {
                    message.Parameter = message.Text.Substring(Constants.SpecialCommands.EnterCode.Length);
                    return ProcessingManager.EnterCode(message);
                }
                var waiting4List = ConversationManager.WaitingList.Where(x => x.chatId == message.ChatId && x.sender == message.SenderId)?.ToList();
                var waiting4 = (waiting4List == null || !waiting4List.Any()) ? ConversationManager.WaitingReason.None : waiting4List.First().waitingFor;
                switch(waiting4)
                {
                    case ConversationManager.WaitingReason.None:
                        break;
                    case ConversationManager.WaitingReason.GameUrl:
                        var result = ProcessingManager.StartGameByUrl(message);
                        if (result != null) return result;
                        break;
                    default:
                        break;
                }
                var chatPrefix = StaticData.Prefixes.Get(message.ChatId);
                if (chatPrefix != null 
                    && message.Text != null
                    && message.Text.Length > chatPrefix.Length)
                {
                    if (chatPrefix == message.Text.Substring(0,chatPrefix.Length))
                    {
                        message.Parameter = message.Text;
                        return ProcessingManager.EnterCode(message);
                    }
                }
                if (message.Coordinates != null)
                {
                    return new ProcessingResult()
                    {
                        ChatId = message.ChatId,
                        ReplyTo = message.Id,
                        DisableWebPagePreview = true,
                        Markup = Constants.Keyboards.CoordinatesKeyboard(message.Coordinates),
                        Text = $"{message.Coordinates.Lat},{message.Coordinates.Lon}"
                    };
                }
                var location = Coordinates.ParseCoordinates(message.Text);
                if (location != null)
                {
                    return ProcessingResult.CreateCoordinates(message, location);
                }
                if (message.ReplyToBot)
                {
                    return ConversationManager.Process(message);
                }
            }
            catch(Exception ex)
            {
                //switch (ex.Level)
                //{
                //    case GCException.LevelType.Fatal:
                //        throw new Exception(ex.Message);
                //    case GCException.LevelType.Chat:
                //        return ProcessingResult.CreateText(message, ex.Message);
                //    case GCException.LevelType.ChatFull:
                //        return ProcessingResult.CreateText(message, $"Code:{ex.Code}, Message:{ex.Message}, Stack:{ex.StackTrace}");
                //    case GCException.LevelType.Log:
                //        Log.New(ex, message);
                //        break;
                //    case GCException.LevelType.Quiet:
                //        break;
                //}
                //return ProcessingResult.CreateText(message, $"Message: {ex.Message??""}; StackTrace: {ex.StackTrace??""};");
            }
            //return ProcessingResult.CreateText(message, "Nothing to see here");
            return null;
        }
        
    }
}
