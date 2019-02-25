using System;
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
        public static ProcessingResult? Process(string command, List<string> parameters, long chatId)
        {
            Game game;
            IGameEngine engine;

            switch (command.ToLower())
            {
                case Constants.Commands.GameCommands.StartGame:
                    return StartGame(parameters, chatId);

                case Constants.Commands.GameCommands.JoinGame:
                    if (parameters?.Count != 1) return null;
                    return JoinGame(parameters[0], chatId);

                case Constants.Commands.GameCommands.ExitFromGame:
                    GameManager.ExitFromCurrentGame(chatId);
                    return null;
                case Constants.Commands.GameCommands.GetStatistics:
                    return GetStatistics(chatId);
                case Constants.Commands.GameCommands.SetAuth:
                    if (parameters == null || parameters.Count < 2)
                    {
                        return new ProcessingResult("enter auth info", chatId);
                    }
                    //save
                    GameManager.SetAuthToActiveGame(parameters[0], parameters[1], chatId);
                    game = GameManager.GetActiveGameByChatId(chatId);
                    engine = IGameEngine.Get(game, chatId);
                    //check if can connect
                    if (!engine.Login())
                    {
                        //delete wrong auth data
                        GameManager.SetAuthToActiveGame(null, null, chatId);
                    }
                    
                    return null;

                case Constants.Commands.GameCommands.GetTask:
                    game = GameManager.GetActiveGameByChatId(chatId);
                    engine = IGameEngine.Get(game, chatId);
                    var task = engine.GetTask();
                    return (task == null) ? null : new ProcessingResult(task, chatId);

                case Constants.Commands.GameCommands.SetTaskMonitoringStatus:
                    int status;
                    if (parameters == null || parameters.Count != 1 || !int.TryParse(parameters[0], out status) || status > 2)
                        return new ProcessingResult(" 0 - no notifications; 1 - only change notification; 2 - with text", chatId);
                    GameManager.SetTaskMonitoring(chatId, status);
                    return null;

                case Constants.Commands.GameCommands.EndGame:
                    return EndGame(chatId);

                case Constants.Commands.GameCommands.Game:
                    return GameStatus(chatId);
                default:
                    return null;
            }
        }

        private static ProcessingResult GetStatistics(long chatId)
        {
            Game game = GameManager.GetActiveGameByChatId(chatId);
            var engine = IGameEngine.Get(game, chatId);
            var result = new ProcessingResult("stats", chatId)
            {
                Image = engine.GetStatistics()
            };
            return result;
        }

        private static ProcessingResult? EndGame(long chatId)
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
                    result = new ProcessingResult(String.Format("You are in active game. ({0} - {1})\r\n  Please, set auth for the game",game.Type.ToString(),game.Id), chatId, Constants.Keyboards.NoAuthGame, true, false, null);
                }
                else //game.Login == null
                {
                    if (game.Type == GameType.EnCx && String.IsNullOrEmpty(game.EnCxId))
                    {
                        var activeGames = (new EnCxQuestEngine(chatId)).GetAllEnCxActiveGames();
                        var keyboardData = new List<List<InlineKeyboardButton>>();
                        foreach (var activeGame in activeGames)
                        {
                            keyboardData.Add(new List<InlineKeyboardButton>()
                            {
                                new InlineKeyboardButton()
                                {
                                    Text = activeGame.Title,
                                    CallbackData = String.Format("EnCxActiveGame|{0}", activeGame.EnCxId)
                                }
                            });
                        }
                        result = new ProcessingResult("Choose Active Game:", chatId, new InlineKeyboardMarkup(keyboardData), true, false, null);
                    }
                    else
                    {
                        var participant = game.Participants.Where(x => x.ChatId == chatId).FirstOrDefault();
                        result = new ProcessingResult("Game settings:", chatId, Constants.Keyboards.ActiveGame(participant.TaskMonitoring, 0), true, false, null);
                    }
                }
            }
            return result;
        }

        private static ProcessingResult? JoinGame(string gameId, long chatId)
        {
            var result = GameManager.JoinGame(gameId, chatId);
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
