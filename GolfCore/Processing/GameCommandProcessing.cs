﻿using System;
using System.Collections.Generic;
using System.Text;
using GolfCoreDB.Data;
using GolfCoreDB.Managers;
using GolfCore.GameEngines;
using System.Linq;

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

                case "exitfromgame":
                    GameManager.ExitFromCurrentGame(chatId);
                    return null;

                case "setauth":
                    if (parameters == null || parameters.Count < 2)
                    {
                        return new ProcessingResult("enter auth info", chatId);
                    }
                    GameManager.SetAuthToActiveGame(parameters[0], parameters[1], chatId);
                    return null;

                case "gettask":
                    Game game = GameManager.GetActiveGameByChatId(chatId);
                    if (game.Type == GameType.IgraLv)
                    {
                        IgraLvGameEngine engine = new IgraLvGameEngine(chatId);
                        return new ProcessingResult(engine.GetTask(), chatId);
                        //return engine.GetTask();
                    }
                    return null;

                case "settaskmonitoring":
                    int status;
                    if (parameters == null || parameters.Count != 1 || !int.TryParse(parameters[0], out status) || status > 2)
                        return new ProcessingResult(" 0 - no notifications; 1 - only change notification; 2 - with text", chatId);
                    GameManager.SetTaskMonitoring(chatId, status);
                    return null;
                default:
                    return null;
            }
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
