using GolfCore.GameEngines;
using GolfCoreDB.Data;
using GolfCoreDB.Managers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace GolfCore.Processing
{
    public class CallBackProcessing
    {
        public static ProcessingResult? Process(string data, long chatId, bool isPrivate, long sender, out ProcessingResult? editMessage)
        {
            ProcessingResult? result = null;
            ProcessingResult? edit = null;
            Game game;
            IGameEngine engine;
            string? resultText;

            switch(data)
            {
                case "":
                    break;
                case "CreateEncx":
                    resultText = GameManager.CreateGame(GameType.EnCx, chatId);
                    result = new ProcessingResult(resultText, chatId);
                    edit = GameCommandProcessing.GameStatus(chatId);
                    break;
                case "CreateDemo":
                    resultText = GameManager.CreateGame(GameType.Demo, chatId);
                    result = new ProcessingResult(resultText, chatId);
                    edit = GameCommandProcessing.GameStatus(chatId);
                    break;
                case "CreateLvlUp":
                    resultText = GameManager.CreateGame(GameType.LvlUp, chatId);
                    result = new ProcessingResult(resultText, chatId);
                    edit = GameCommandProcessing.GameStatus(chatId);
                    break;
                case "CreateIgra":
                    resultText = GameManager.CreateGame(GameType.IgraLv, chatId);
                    //result = new ProcessingResult(resultText, chatId);
                    result = null;
                    edit = GameCommandProcessing.GameStatus(chatId);
                    break;
                case "SetAuth":
                    if (isPrivate)
                    {
                        result = new ProcessingResult(Constants.Text.SetAuthInPrivate, chatId, new ForceReplyMarkup());
                    }
                    else
                    {
                        var currentGame = GameManager.GetActiveGameByChatId(chatId);
                        GameManager.JoinGame(currentGame.Id, sender, true);
                        result = new ProcessingResult(Constants.Text.SetAuthInGroup, sender,new ForceReplyMarkup());
                    }
                    break;
                case "Joinlink":
                    game = GameManager.GetActiveGameByChatId(chatId);
                    result = new ProcessingResult($"/{Constants.Commands.GameCommands.JoinGame}_{game.Id}", chatId);
                    break;
                case "ExitGame":
                    GameManager.ExitFromCurrentGame(chatId);
                    edit = GameCommandProcessing.GameStatus(chatId);
                    break;
                case "GetTask":
                    game = GameManager.GetActiveGameByChatId(chatId);
                    engine = IGameEngine.Get(game, chatId);
                    var task = engine.GetTask();
                    result = (task == null) ? null : new ProcessingResult(task, chatId);
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
                case "GetStatistics":
                    game = GameManager.GetActiveGameByChatId(chatId);
                    engine = IGameEngine.Get(game, chatId);
                    Image<Rgba32>? stat = engine.GetStatistics();
                    result = (stat == null) ? null : new ProcessingResult("Stats", chatId)
                    {
                        Image = stat
                    };
                    break;
                case "DisableStatisticUpdates":
                case "EnableStatisticLvl":
                case "EnableStatistics":
                    break;
                default:
                    if (data.Contains("|"))
                    {
                        game = GameManager.GetActiveGameByChatId(chatId);
                        engine = IGameEngine.Get(game, chatId);
                        GameManager.GetEnCxType(game.EnCxType, out var entype, out var enstatus, out var enzone);
                        var command = data.Split("|")[0];
                        var parameter = data.Split("|")[1];
                        switch(command)
                        {
                            //case "EnCxActiveGame":
                            //    var activeGames = (engine as IEnCxEngine).GetAllEnCxActiveGames();
                            //    var activeGame = activeGames.Where(x => x.EnCxId == parameter).FirstOrDefault();
                            //    game = GameManager.GetActiveGameByChatId(chatId);
                            //    game.EnCxId = activeGame.EnCxId;
                            //    game.Title = activeGame.Title;
                            //    game.Href = activeGame.Href;
                            //    GameManager.UpdateGame(game);
                            //    edit = GameCommandProcessing.GameStatus(chatId);
                            //    break;
                            case "EnCxZone":
                                if (!String.IsNullOrWhiteSpace(enzone))
                                {
                                    editMessage = null;
                                    return null;
                                }
                                game.EnCxType = GameManager.SetEnCxType(entype, enstatus, parameter);
                                GameManager.UpdateGame(game);
                                edit = GameCommandProcessing.GameStatus(chatId);
                                break;
                            case "EnCxType":
                                if (!String.IsNullOrWhiteSpace(entype))
                                {
                                    editMessage = null;
                                    return null;
                                }
                                game.EnCxType = GameManager.SetEnCxType(parameter, enstatus, enzone);
                                GameManager.UpdateGame(game);
                                edit = GameCommandProcessing.GameStatus(chatId);
                                break;
                            case "EnCxStatus":
                                if (!String.IsNullOrWhiteSpace(enstatus))
                                {
                                    editMessage = null;
                                    return null;
                                }
                                game.EnCxType = GameManager.SetEnCxType(entype, parameter, enzone);
                                GameManager.UpdateGame(game);
                                edit = GameCommandProcessing.GameStatus(chatId);
                                break;
                            case Constants.Buttons.CancelButtonCallbackText:
                                game.EnCxType = GameManager.SetEnCxType(
                                    (parameter == "type") ? null :entype
                                    ,(parameter == "game") ? null : enstatus
                                    ,(parameter == "zone") ? null : enzone
                                    );
                                GameManager.UpdateGame(game);
                                edit = GameCommandProcessing.GameStatus(chatId);
                                break;
                            case "EnCxGame":
                                game.EnCxId = parameter;
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
