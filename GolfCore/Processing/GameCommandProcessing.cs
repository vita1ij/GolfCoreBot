﻿using System;
using System.Collections.Generic;
using System.Text;
using GolfCoreDB.Data;
using GolfCoreDB.Managers;
using GolfCore.GameEngines;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace GolfCore.Processing
{
    public class GameCommandProcessing
    {
        public static ProcessingResult Process(string command, List<string> parameters, long chatId)
        {
            switch (command.ToLower())
            {
                case Constants.Commands.StartGame:
                    return StartGame(parameters, chatId);

                case Constants.Commands.JoinGame:
                    return JoinGame(parameters, chatId);

                case Constants.Commands.ExitFromGame:
                    GameManager.ExitFromCurrentGame(chatId);
                    return null;

                case Constants.Commands.SetAuth:
                    if (parameters == null || parameters.Count < 2)
                    {
                        return new ProcessingResult("enter auth info", chatId);
                    }
                    GameManager.SetAuthToActiveGame(parameters[0], parameters[1], chatId);
                    return null;

                case Constants.Commands.GetTask:
                    Game game = GameManager.GetActiveGameByChatId(chatId);
                    IGameEngine engine = IGameEngine.Get(game, chatId);
                    return new ProcessingResult(engine.GetTask(), chatId);

                case Constants.Commands.SetTaskMonitoringStatus:
                    int status;
                    if (parameters == null || parameters.Count != 1 || !int.TryParse(parameters[0], out status) || status > 2)
                        return new ProcessingResult(" 0 - no notifications; 1 - only change notification; 2 - with text", chatId);
                    GameManager.SetTaskMonitoring(chatId, status);
                    return null;

                case Constants.Commands.EndGame:
                    return EndGame(chatId);

                case Constants.Commands.Game:
                    return GameStatus(chatId);
                default:
                    return null;
            }
        }

        private static ProcessingResult EndGame(long chatId)
        {
            GameManager.EndGame(chatId);
            return null;
        }

        public static ProcessingResult GameStatus(long chatId)
        {
            Game game = GameManager.GetActiveGameByChatId(chatId);
            ProcessingResult result;

            if (game == null )
            {
                result = new ProcessingResult("No active Games. Feel free to create one.", chatId, Constants.Keyboards.NoActiveGame, true, false, null);
            }
            else //(game != null)
            {
                if (game.Login == null)
                {
                    result = new ProcessingResult("Please, set auth for the game", chatId, Constants.Keyboards.NoAuthGame, true, false, null);
                }
                else //game.Login == null
                {
                    var participant = game.Participants.Where(x => x.ChatId == chatId).FirstOrDefault();
                    result = new ProcessingResult("Game settings:", chatId, Constants.Keyboards.ActiveGame(participant.TaskMonitoring, 0), true, false, null);
                }
            }
            return result;
        }

        private static ProcessingResult JoinGame(List<string> list, long chatId)
        {
            string result = null;
            if (list.Any())
            {
                result = GameManager.JoinGame(list[0], chatId);
            }
            return new ProcessingResult(result, chatId);
        }

        private static ProcessingResult StartGame(List<string> list, long chatId)
        {
            if (list == null || !list.Any())
            {
                return new ProcessingResult("Dont forget game type: igra/encx", chatId);
            }

            GameType type;
            switch (list[0].ToLower())
            {
                case "igra":
                    type = GameType.IgraLv;
                    break;
                case "encx":
                    type = GameType.EnCx;
                    break;
                default:
                    return new ProcessingResult("unknown gametype. try igra/encx", chatId);
            }
            string result = GameManager.CreateGame(type, chatId);

            return new ProcessingResult(result, chatId);
        }
    }
}
