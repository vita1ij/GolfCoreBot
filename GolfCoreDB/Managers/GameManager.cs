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
                var game = db.Games.Where(x => x.Id == gameId);
                if (game == null || !game.Any())
                {
                    return "no such game";
                }
                if (game.First().Participants == null)
                {
                    game.First().Participants = new List<GameParticipant>();
                }
                (game.First().Participants).Add(new GameParticipant()
                {
                    ChatId = chatId,
                    Game = game.First()
                });
                db.SaveChanges();
                return "Joined";
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

        ///// <param name="status">
        ///// 0 - no
        ///// 1 - only change notification
        ///// 2 - with text
        ///// </param>
        //public static void SetTaskMonitoring(string gameId, long chatId, int status)
        //{
        //    using (var db = DBContext.Instance)
        //    {
        //        var games = db.Games.Where(x => x.Id == gameId).Include("Participants");
        //        if (games != null && games.Any())
        //        {
        //            var participants = games.ToList().SelectMany<Game, GameParticipant>(x => x.Participants);
        //            var par = participants.Where(x => x.ChatId == chatId);
        //            if (par != null && par.Any())
        //            {
        //                foreach(var p in par)
        //                {
        //                    p.MonitorUpdates = (status > 0);
        //                    p.GetUpdates = (status == 2);
        //                }
        //            }
        //        }
        //    }
        //}

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

        public static void GetAuthFromToActiveGame(long chatId, out string login, out string pass)
        {
            using (var db = DBContext.Instance)
            {
                var game = xGetActiveGameByChatId(chatId, db);
                login = game.Login;
                pass = game.Password;
            }

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

        //public static string F
    }
}
