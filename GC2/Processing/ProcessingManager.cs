using GC2.Helpers;
using GC2DB.Data;
using GC2DB.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using static GC2.GCException;
using Telegram.Bot.Types.ReplyMarkups;
using GC2.Engines;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using GC2.Classes;

namespace GC2
{
    public class ProcessingManager
    {
        private static IConfiguration Config
        {
            get
            {
                var c = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("config.json");
                return c.Build();
            }
        }

        public static ProcessingResult Foo(ReceivedMessage message)
        {
            return new ProcessingResult()
            {
                ChatId = message.ChatId,
                Text = "bar",
                Markup = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup(new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> {
                        new KeyboardButton("q")
                        ,new KeyboardButton("w")
                        ,new KeyboardButton("e")
                    }
                    , new List<KeyboardButton> {
                        new KeyboardButton("a")
                        ,new KeyboardButton("s")
                        ,new KeyboardButton("d")}
                    , new List<KeyboardButton> {
                        new KeyboardButton("z")
                        ,new KeyboardButton("x")
                        ,new KeyboardButton("c")}
                })
            };
        }

        public static ProcessingResult ProcessAddress(ReceivedMessage message)
        {
            var input = message.Parameter;
            if (String.IsNullOrEmpty(input)) return null;

            var coordinates = Coordinates.ParseCoordinates(input);

            if (coordinates != null)
            {
                var address = LocationsHelper.GetAddress(coordinates);
                if (address != null)
                {
                    return ProcessingResult.CreateReply(message, address);
                }
            }
            else
            {
                try
                {
                    var cords = LocationsHelper.GetCoordinates(message.Parameter);
                    if (cords != null)
                    {
                        return ProcessingResult.CreateHtml(message, cords);
                    }
                }
                catch (Exception ex)
                {
                    Log.New(ex);
                }
            }

            return null;
        }

        internal static ProcessingResult SaveCoordinates(ReceivedMessage message)
        {
            return null;
        }

        internal static ProcessingResult GameSetup(ReceivedMessage message)
        {
            ProcessingResult result = null;
            var activeGame = GameManager.GetActiveGameByChatId(message.ChatId);
            if (activeGame == null)
            {
                result = new ProcessingResult()
                {
                    ChatId = message.ChatId,
                    Text = Constants.Replies.NO_ACTIVE_GAME_CREATE,
                    Markup = Constants.Keyboards.CreateNewGame,
                };
            }
            else if (activeGame.Login == null || activeGame.Password == null)
            {
                if (IGameEngine.Get(activeGame) is IEnCxGameEngine && activeGame.EnCxId == null)
                {
                    result = FindEnCxGame(message);
                }
                else
                {
                    var _markup = Constants.Keyboards.GameWithoutAuth(activeGame);
                    var loginsConf = Config.GetSection("LOGINS")
                                        ?.GetSection(message.ChatId.ToString())
                                        ?.GetSection(activeGame.Type.ToString());
                    if (loginsConf != null)
                    {
                        var logins = loginsConf.GetChildren();
                        if (logins != null && logins.ToList().Any())
                        {
                            var buttons = _markup.InlineKeyboard.ToList();
                            buttons.AddRange(
                                logins.Select(x => new List<InlineKeyboardButton> { new InlineKeyboardButton { Text = $"Login as: {x.Key}", CallbackData = $"/{Constants.Commands.SetAuth} {x.Key}" } }).ToList()
                            );
                            _markup = new InlineKeyboardMarkup(buttons);
                        }
                    }
                    result = new ProcessingResult()
                    {
                        ChatId = message.ChatId,
                        Text = String.Format(Constants.Replies.GAME_NO_AUTH, activeGame.Guid, activeGame.Type.ToString()),
                        Markup = _markup
                    };
                }
            }
            else
            {
                result = new ProcessingResult()
                {
                    ChatId = message.ChatId,
                    Text = Constants.Replies.GAME_FULL,
                    Markup = Constants.Keyboards.GameCommands(activeGame)
                };
            }

            if (result != null && message.IsCallback)
            {
                result.MessageId = message.Id;
                result = new ProcessingResult()
                {
                    EditMessages = new List<ProcessingResult>
                    {
                        result
                    }
                };
            }

            if (result != null && message.Command == Constants.Commands.GameSetup)
            {
                result.Delete = true;
                result.MessageId = message.Id;
            }

            return result;
        }

        internal static ProcessingResult EnterCode(ReceivedMessage message)
        {
            var player = GameManager.GetActivePlayer(message.ChatId);
            var activeGame = GameManager.GetActiveGameByChatId(message.ChatId);
            if (player == null || activeGame == null || String.IsNullOrWhiteSpace(message.Parameter)) return null;

            var engine = IGameEngine.Get(activeGame);

            var result = engine.EnterCode(message.Parameter, activeGame);
            return ProcessingResult.CreateText(message, result.ToString());
        }

        internal static ProcessingResult SetTaskUpdate(ReceivedMessage message)
        {
            var player = GameManager.GetActivePlayer(message.ChatId);
            if (player == null) return null;
            switch(message.Command)
            {
                case Constants.Commands.GameTaskNoUpdates:
                    player.UpdateTaskInfo = Player.PlayerUpdateStatusInfo.DontUpdate;
                    break;
                case Constants.Commands.GameTaskUpdateStatus:
                    player.UpdateTaskInfo = Player.PlayerUpdateStatusInfo.UpdateStatus;
                    break;
                case Constants.Commands.GameTaskUpdateText:
                    player.UpdateTaskInfo = Player.PlayerUpdateStatusInfo.UpdateText;
                    break;
            }
            GameManager.UpdatePlayer(player);
            return null;
        }

        internal static ProcessingResult SetRadius(ReceivedMessage message)
        {
            if (message.Parameters == null || !message.Parameters.Any()) return null;

            if (message.Parameters?.Count < 2)
            {
                return new ProcessingResult
                {
                    Text = String.Format(Constants.Replies.SET_RADIUS_FORMAT, message.Parameters[0]),
                    ChatId = message.SenderId,
                    Markup = new ForceReplyMarkup(),
                    MessageId = message.Id
                };
            }
            else
            {
                if (long.TryParse(message.Parameters[0], out var gameId) && long.TryParse(message.Parameters[1], out var distance))
                {
                    var activeGame = GameManager.GetById(gameId);
                    if (activeGame != null)
                    {
                        activeGame.Radius = distance;
                        GameManager.Update(activeGame);
                        return new ProcessingResult
                        {
                            Delete = true,
                            MessageId = message.Id,
                            ChatId = message.ChatId
                        };
                    }
                }
            }
            return null;
        }

        internal static ProcessingResult SetPrefix(ReceivedMessage message)
        {
            if (message.Parameters == null || !message.Parameters.Any()) return null;

            if (message.Parameters?.Count < 2)
            {
                return new ProcessingResult
                {
                    Text = String.Format(Constants.Replies.SET_PREFIX_FORMAT, message.Parameters[0]),
                    ChatId = message.SenderId,
                    Markup = new ForceReplyMarkup(),
                    MessageId = message.Id
                };
            }
            else
            {
                if (long.TryParse(message.Parameters[0], out var gameId))
                {
                    var activeGame = GameManager.GetById(gameId);
                    if (activeGame != null)
                    {
                        activeGame.Prefix = message.Parameters[1];
                        GameManager.Update(activeGame);
                        return new ProcessingResult
                        {
                            Delete = true,
                            MessageId = message.Id,
                            ChatId = message.ChatId
                        };
                    }
                }
            }
            return null;
        }

        internal static ProcessingResult FoundEncxGame(ReceivedMessage message)
        {
            if (String.IsNullOrEmpty(message.Parameter)) return null;

            var activeGame = GameManager.GetActiveGameByChatId(message.ChatId);
            if (activeGame == null) return null;
            activeGame.EnCxId = message.Parameter;
            GameManager.Update(activeGame);

            return GameSetup(message);
        }

        internal static ProcessingResult FindEnCxGame(ReceivedMessage message)
        {
            if (message.Parameter == null || message.Parameters == null || message.Parameters.Count == 0)
            {
                return new ProcessingResult
                {
                    ChatId = message.ChatId,
                    Text = "choose zone",
                    Markup = Constants.Keyboards.GameEnCxZone,
                    ReplaceOriginal = message.IsCallback
                };
            }

            switch (message.Parameters?.Count)
            {
                case 1:
                    return new ProcessingResult
                    {
                        ChatId = message.ChatId,
                        Text = "choose status",
                        Markup = Constants.Keyboards.GameEnCxStatus(message.Parameter),
                        ReplaceOriginal = message.IsCallback
                    };
                case 2:
                    return new ProcessingResult
                    {
                        ChatId = message.ChatId,
                        Text = "choose type",
                        Markup = Constants.Keyboards.GameEnCxType(message.Parameters[0],message.Parameters[1]),
                        ReplaceOriginal = message.IsCallback
                    };
                case 3:
                    var activeGame = GameManager.GetActiveGameByChatId(message.ChatId);
                    IEnCxGameEngine engine = IGameEngine.Get(activeGame) as IEnCxGameEngine;
                    var games = engine.GetEnCxGames(message.Parameters[0], message.Parameters[1], message.Parameters[2]);
                    return new ProcessingResult
                    {
                        ChatId = message.ChatId,
                        Text = "choose game",
                        Markup = Constants.Keyboards.GameEnCxGames(games),
                        ReplaceOriginal = message.IsCallback
                    };
                default:
                    return ProcessingResult.CreateText(message, "later");
            }
        }

        internal static ProcessingResult GetTask(ReceivedMessage message)
        {
            var activeGame = GameManager.GetActiveGameByChatId(message.ChatId);
            if (activeGame == null) return null;

            var engine = IGameEngine.Get(activeGame);
            var task = engine.GetTask(out var stuff);
            var result = ProcessingResult.CreateHtml(message, task.Text);
            if (stuff != null)
            {
                result.Images = new List<ImageResult>();
                foreach(ImageResult img in stuff.Where(x => x is ImageResult))
                {
                    result.Images.Add(img);
                }
            }
            return result;
        }

        internal static ProcessingResult ShowGameSettings(ReceivedMessage message)
        {
            var activeGame = GameManager.GetActiveGameByChatId(message.ChatId);
            if (activeGame == null) return null;

            return new ProcessingResult
            {
                EditMessages = new List<ProcessingResult>
                {
                    new ProcessingResult
                    {
                        ChatId = message.ChatId,
                        Text = Constants.Replies.GAME_FULL,
                        Markup = Constants.Keyboards.GameSettingsAndCommands(activeGame),
                        MessageId = message.Id
                    }
                }
            };
        }

        internal static ProcessingResult JoinGame(ReceivedMessage message)
        {
            if (message.Parameter == null) return null;
            GameManager.JoinGame(null, message.Parameter, message.ChatId, true);
            var result = GameSetup(message);
            if (result == null) return null;
            result.Delete = true;
            result.MessageId = message.Id;
            return result;
        }

        internal static ProcessingResult StartSetingAuth(ReceivedMessage message)
        {
            var activeGame = GameManager.GetActiveGameByChatId(message.ChatId);
            if (activeGame == null)
            {
                throw new GCException(Constants.Exceptions.ExceptionCode.NoActiveGame, LevelType.Chat);
            }
            if (String.IsNullOrEmpty(message.Parameter))
            {
                //has nothing
                if (!message.PrivateChat)
                {
                    //send to private
                    GameManager.JoinGame(activeGame.Id, null, message.ChatId, true);
                    return new ProcessingResult()
                    {
                        ChatId = message.SenderId,
                        Text = String.Format(Constants.Replies.AUTH_GET_LOGIN_FROM_PUBLIC, activeGame.Guid),
                    };
                }
                else
                {
                    return new ProcessingResult()
                    {
                        ChatId = message.ChatId,
                        Text = String.Format(Constants.Replies.AUTH_GET_LOGIN, activeGame.Guid),
                        Markup = new ForceReplyMarkup()
                    };
                }
            }
            else if (message.Parameters != null && message.Parameters.Count == 1)
            {
                var loginsConf = Config.GetSection("LOGINS")
                                        ?.GetSection(message.ChatId.ToString())
                                        ?.GetSection(activeGame.Type.ToString());
                var logins = loginsConf?.GetChildren();
                if (logins != null && logins.ToList().Any(x => x.Key == message.Parameter))
                {
                    message.Parameters.Add(logins.Where(x => x.Key == message.Parameter).First().Value);
                }
                //has login
                else return new ProcessingResult()
                {
                    ChatId = message.ChatId,
                    Text = String.Format(Constants.Replies.AUTH_GET_PASSWORD, activeGame.Guid, message.Parameter),
                    Markup = new ForceReplyMarkup(),
                    Delete = true,
                    MessageId = message.Id
                };
            }
            if (message.Parameters != null && message.Parameters.Count == 2)
            {
                //has login and pass
                activeGame.Login = message.Parameters[0];
                activeGame.Password = message.Parameters[1];
                var engine = IGameEngine.Get(activeGame);
                try
                {
                    if (engine.Login(activeGame))
                    {
                        //activeGame.Update();
                        GameManager.Update(activeGame);
                    }
                    
                }
                catch(Exception ex)
                {

                }

                return new ProcessingResult
                {
                    ChatId = message.ChatId,
                    Delete = true,
                    MessageId = message.Id
                };

            }
            return null;
        }

        internal static ProcessingResult CreateGame(ReceivedMessage message, GameType type)
        {
            if (GameManager.GetActiveGameByChatId(message.ChatId) != null)
            {
                throw new GCException(Constants.Exceptions.ExceptionCode.UniqueGame4Chat, LevelType.Chat);
            }
            GameManager.CreateGame(message.ChatId, type);
            return GameSetup(message);
        }

        internal static ProcessingResult ExitFromGame(ReceivedMessage message)
        {
            GameManager.ExitFromGame(message.ChatId);
            return GameSetup(message);
        }
    }
}
