using GolfCore.GameEngines;
using GolfCoreDB.Data;
using GolfCoreDB.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GolfCore.Processing
{
    public class CallBackProcessing
    {
        public static ProcessingResult Process(string data, long chatId, bool isPrivate, out ProcessingResult editMessage)
        {
            ProcessingResult result = null;
            ProcessingResult edit = null;
            Game game = null;
            IGameEngine engine = null;
            GameParticipant participant = null;
            string res;

            switch(data)
            {
                case "":
                    break;
                case "CreateEncx":
                    res = GameManager.CreateGame(GameType.EnCx, chatId);
                    result = new ProcessingResult(res, chatId);
                    edit = GameCommandProcessing.GameStatus(chatId);
                    break;
                case "CreateIgra":
                    res = GameManager.CreateGame(GameType.IgraLv, chatId);
                    result = new ProcessingResult(res, chatId);
                    edit = GameCommandProcessing.GameStatus(chatId);
                    break;
                case "SetAuth":
                    if (isPrivate)
                    {
                        result = new ProcessingResult(Constants.Text.SetAuthInPrivate, chatId);
                    }
                    else
                    {
                        result = new ProcessingResult(Constants.Text.SetAuthInGroup, chatId);
                    }
                    break;
                case "Joinlink":
                    game = GameManager.GetActiveGameByChatId(chatId);
                    result = new ProcessingResult($"/{Constants.Commands.JoinGame}_{game.Id}", chatId);
                    break;
                case "ExitGame":
                    GameManager.ExitFromCurrentGame(chatId);
                    edit = GameCommandProcessing.GameStatus(chatId);
                    break;
                case "GetTask":
                    game = GameManager.GetActiveGameByChatId(chatId);
                    engine = IGameEngine.Get(game, chatId);
                    result = new ProcessingResult(engine.GetTask(), chatId);
                    break;
                case "DisableTaskUpdates":
                    game = GameManager.GetActiveGameByChatId(chatId);
                    GameManager.SetTaskMonitoring(chatId, 0);
                    edit = GameCommandProcessing.GameStatus(chatId);
                    break;
                case "EnableTaskUpdates":
                    game = GameManager.GetActiveGameByChatId(chatId);
                    GameManager.SetTaskMonitoring(chatId, 1);
                    edit = GameCommandProcessing.GameStatus(chatId);
                    break;
                case "EnableTaskTask":
                    game = GameManager.GetActiveGameByChatId(chatId);
                    GameManager.SetTaskMonitoring(chatId, 2);
                    edit = GameCommandProcessing.GameStatus(chatId);
                    break;
                case "DisableStatisticUpdates":
                case "EnableStatisticLvl":
                case "EnableStatistics":
                    break;
                default:
                    if (data.Contains("|"))
                    {
                        var command = data.Split("|")[0];
                        var parameter = data.Split("|")[1];
                        switch(command)
                        {
                            case "EnCxActiveGame":
                                var activeGames = (new EnCxQuestEngine(chatId)).GetAllEnCxActiveGames();
                                var activeGame = activeGames.Where(x => x.EnCxId == parameter).FirstOrDefault();
                                game = GameManager.GetActiveGameByChatId(chatId);
                                game.EnCxId = activeGame.EnCxId;
                                game.Title = activeGame.Title;
                                game.Href = activeGame.Href;
                                GameManager.UpdateGame(game);
                                edit = GameCommandProcessing.GameStatus(chatId);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
            }

            editMessage = edit;
            return result;
        }
    }
}
