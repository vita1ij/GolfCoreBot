using GC2.Helpers;
using GC2DB.Data;
using GC2DB.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace GC2
{
    public class Processing
    {
        public static ProcessingResult? ProcessMessage(ReceivedMessage message)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(message.Command))
                {
                    switch (message.Command)
                    {
                        case "foo":
                            return ProcessingManager.Foo(message);
                        case Constants.Commands.Help:
                        case Constants.Commands.HelpShort:
                            return ProcessingResult.CreateText(message, Constants.Replies.HELP);
                        case Constants.Commands.Address:
                            return ProcessingManager.ProcessAddress(message);
                        case Constants.Commands.List:
                        case Constants.Commands.ListShort:
                            return ProcessingResult.CreateText(message, ListManager.GetList(message.ChatId));
                        case Constants.Commands.ListSorted:
                        case Constants.Commands.ListSortedShort:
                            return ProcessingResult.CreateText(message, ListManager.GetList(message.ChatId, true));
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
                        case Constants.Commands.CreateEnCxGame:
                            return ProcessingManager.CreateGame(message, GameType.EnCx);
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
                        default:
                            break;
                    }
                }
                if (message.Text != null && message.Text.StartsWith(Constants.SpecialCommands.AddListValue))
                {
                    ListManager.AddValue(message.ChatId, message.Text.Substring(Constants.SpecialCommands.AddListValue.Length));
                    return null;
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
            catch(GCException ex)
            {
                switch (ex.Level)
                {
                    case GCException.LevelType.Fatal:
                        throw new Exception(ex.Message);
                    case GCException.LevelType.Chat:
                        return ProcessingResult.CreateText(message, ex.Message);
                    case GCException.LevelType.ChatFull:
                        return ProcessingResult.CreateText(message, $"Code:{ex.Code}, Message:{ex.Message}, Stack:{ex.StackTrace}");
                    case GCException.LevelType.Log:
                        Log.New(ex, message);
                        break;
                    case GCException.LevelType.Quiet:
                        break;
                }
            }
            return null;
        }
        
    }
}
