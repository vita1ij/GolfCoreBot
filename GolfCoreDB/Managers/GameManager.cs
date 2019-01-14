using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GolfCoreDB.Data;
using Microsoft.EntityFrameworkCore;

namespace GolfCoreDB.Managers
{
    public static class GameManager
    {
        public static Game GetActiveGameByChatId(long chatId)
        {
            using (var db = DBContext.Instance)
            {
                return xGetActiveGameByChatId(chatId, db);
            }
        }

        public static Game xGetActiveGameByChatId(long chatId, DBContext db)
        {
            var games = db.Games.Where(x => x.Participants.Any(y => y.ChatId == chatId) && x.IsActive).Include("Participants");
            return games.FirstOrDefault();
        }

        public static List<Game> GetAllActiveWithMonitoring()
        {
            using (var db = DBContext.Instance)
            {
                return db.Games.Where(x => x.IsActive && x.Participants.Any(y => y.MonitorUpdates)).Include("Participants").ToList();
            }
        }

        public static string CreateGame(GameType type, long chatId, string login = null, string pass = null)
        {
            string newId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);

            using (var db = DBContext.Instance)
            {
                if (db.Games.Where(x => x.IsActive == true && x.Participants.Any(y => y.ChatId == chatId)).Any())
                {
                    return "End existing game";
                }

                var newGame = new Game()
                {
                    Id = newId,
                    Type = type,
                    IsActive = true,
                    LastTask = DateTime.UtcNow.Date.AddMinutes(DateTime.Now.Minute).ToString()
                };

                newGame.Participants = new List<GameParticipant>()
                    {
                        new GameParticipant()
                        {
                            ChatId = chatId,
                            Game = newGame
                        }
                    };

                db.Games.Add(newGame);

                db.SaveChanges();
            }
            return newId;
        }

        public static string JoinGame(string gameId, long chatId)
        {
            using (var db = DBContext.Instance)
            {
                var game = db.Games.Include("Participants").Where(x => x.Id == gameId);
                if (game == null || !game.Any())
                {
                    return "no such game";
                }
                if (game.First().Participants == null)
                {
                    game.First().Participants = new List<GameParticipant>();
                }
                if (game.First().Participants.Exists(x => x.ChatId == chatId))
                {
                    return "You are already joined to game. If you wanna change game, exit from current.";
                }
                else
                {
                    (game.First().Participants).Add(new GameParticipant()
                    {
                        ChatId = chatId,
                        Game = game.First()
                    });
                    db.SaveChanges();
                    return "Joined";
                }
            }
        }

        public static void EndGame(long chatId)
        {
            using (var db = DBContext.Instance)
            {
                var game = xGetActiveGameByChatId(chatId,db);
                game.IsActive = false;
                db.SaveChanges();
            }
        }

        public static string SetAuthToActiveGame(string login, string pass, long chatId)
        {
            using (var db = DBContext.Instance)
            {
                var game = xGetActiveGameByChatId(chatId, db);
                game.Login = login;
                game.Password = pass;
                db.SaveChanges();
                return "Updated Auth";
            }
        }

        /// <param name="status">
        /// 0 - no
        /// 1 - only change notification
        /// 2 - with text
        /// </param>
        public static void SetTaskMonitoring(long chatId, int status)
        {
            using (var db = DBContext.Instance)
            {
                var game = xGetActiveGameByChatId(chatId, db);
                if (game == null) return;
                
                var participants = game.Participants.Where(x => x.ChatId == chatId);
                if (participants != null && participants.Any())
                {
                    foreach (var p in participants)
                    {
                        p.MonitorUpdates = (status > 0);
                        p.GetUpdates = (status == 2);
                    }
                }
                db.SaveChanges();
            }
        }

        public static void GetAuthForActiveGame(long chatId, out string login, out string pass)
        {
            var game = GetActiveGameByChatId(chatId);
            login = game.Login;
            pass = game.Password;

        }

        public static void ExitFromCurrentGame(long chatId)
        {
            using (var db = DBContext.Instance)
            {
                var game = xGetActiveGameByChatId(chatId, db);
                
                game.Participants.RemoveAll(x => x.ChatId == chatId);
                if (!game.Participants.Any())
                {
                    game.IsActive = false;
                }
                db.SaveChanges();
            }
        }

        public static void UpdateGame(Game game)
        {
            using (var db = DBContext.Instance)
            {
                db.Games.Update(game);
                db.SaveChanges();
            }
        }
    }
}
