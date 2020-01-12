using GC2DB.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GC2DB.Managers
{
    public static class GameManager
    {
        private static Player xGetActivePlayer(long chatId, DBContext db)
        {
            return db.Players.FirstOrDefault(x => x.ChatId == chatId && x.IsActive);
        }

        public static Player GetActivePlayer(long chatId)
        {
            using var db = DBContext.Instance;
            return xGetActivePlayer(chatId, db);
        }

        public static void UpdatePlayer(Player p)
        {
            using var db = DBContext.Instance;
            db.Players.Update(p);
            db.SaveChanges();
        }

        private static Game? xGetActiveGameByChatId(long chatId, DBContext db)
        {
            var activePlayer = db.Players
                                    .Include(x => x.Game)
                                    .Where(x => x.ChatId == chatId && x.IsActive == true)
                                    ?.ToList();
            if (activePlayer.Any())
            {
                return activePlayer.First().Game;
            }
            return null;
        }



        public static List<Player> GetAllActivePlayersWithTaskMonitoring()
        {
            using var db = DBContext.Instance;
            return db.Players
                .Where(x => x.IsActive && x.UpdateTaskInfo != Player.PlayerUpdateStatusInfo.DontUpdate)
                .Include(x => x.Game)
                .ToList();
        }

        public static void AddTask(GameTask newTask)
        {
            using var db = DBContext.Instance;
            var maxNumber = db.Tasks.Where(x => x.GameId == newTask.GameId).Max(x => x.Number);

            newTask.Number ??= maxNumber + 1;

            var dbTask = db.Tasks.Add(newTask);
            db.SaveChanges();

            var game = db.Games.Where(x => x.Id == newTask.GameId).First();
            game.LastTaskId = newTask.Id;
            db.Games.Update(game);
            db.SaveChanges();
        }

        public static Game? GetActiveGameByChatId(long chatId)
        {
            using var db = DBContext.Instance;
            return xGetActiveGameByChatId(chatId, db);
        }

        public static Player? GetActivePlayerAndGameByChatId(long chatId)
        {
            using var db = DBContext.Instance;
            var result = db.Players.Where(x => x.ChatId == chatId && x.IsActive)
                .Include(x => x.Game)?.ToList();
            if (result == null || result.Count != 1) return null;
            return result.First();
        }

        public static void CreateGame(long chatId, GameType type)
        {
            using var db = DBContext.Instance;
            var newGame = new Game()
            {
                Type = type
            };
            db.Games.Add(newGame);
            var newPlayer = new Player(newGame, chatId)
            {
                IsActive = true
            };
            db.Players.Add(newPlayer);
            db.SaveChanges();
        }

        private static bool xPlayerInGame(long chatId, long gameId, DBContext db)
        {
            return 
            db.Players
                .Where(x => x.IsActive && x.ChatId == chatId)
                .Include(x => x.Game)
                .Any(x => x.Game.Id == gameId);
        }

        private static void xExitFromGame(long chatId, DBContext db)
        {
            var players = db.Players.Include(x => x.Game)
                    .Where(x => x.ChatId == chatId && x.IsActive)
                    ?.ToList();
            if (players.Any())
            {
                players.First().IsActive = false;
                var activePlayersInGame = db.Players.Include(x => x.Game).Where(x => x.Game == players.First().Game).Count();
                if (activePlayersInGame == 0)
                {
                    players.First().Game.isActive = false;
                }
            }
        }

        public static void ExitFromGame(long chatId)
        {
            using var db = DBContext.Instance;
            xExitFromGame(chatId, db);
            db.SaveChanges();
        }

        public static void JoinGame(long? gameId, string guid, long chatId, bool forceExit = false)
        {
            using var db = DBContext.Instance;
            
            var game = db.Games.Where(x => gameId.HasValue && x.Id == gameId || guid != null && x.Guid == guid).FirstOrDefault(x => x.isActive);
            if (game == null || !game.Id.HasValue) return;

            if (xPlayerInGame(chatId, game.Id.Value, db)) return;
                
            var player = db.Players.Where(x => x.ChatId == chatId).Include(x => x.Game);
            if (player != null && player.Any() && player.First().Game.Id != gameId)
            {
                if (forceExit)
                {
                    //remove from other games
                    xExitFromGame(chatId, db);
                }
            }
            var newPlayer = new Player(game, chatId)
            {
                IsActive = true
            };
            db.Players.Add(newPlayer);
            db.SaveChanges();
        }

        public static void Update(Game game)
        {
            using var db = DBContext.Instance;
            db.Games.Update(game);
            db.SaveChanges();
        }

        public static Game? GetById(long gameId)
        {
            using var db = DBContext.Instance;
            if (db.Games.Any(x => x.Id == gameId))
            {
                return db.Games.Where(x => x.Id == gameId).First();
            }
            return null;
        }

        public static GameTask? GetTaskById(long taskId)
        {
            using var db = DBContext.Instance;
            var result = db.Tasks.Where(x => x.Id == taskId)?.ToList()?.First();
            return result;
        }
    }
}
