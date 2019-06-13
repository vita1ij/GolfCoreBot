using GolfCoreDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GolfCoreDB.Managers
{
    public static class TasksManager
    {
        public static void AddNewTask(GameTask  task)
        {
            using (var db = DBContext.Instance)
            {
                db.Tasks.Add(task);
                db.SaveChanges();
            }
        }

        public static List<GameTask> GetAllTasks(Game game)
        {
            using (var db = DBContext.Instance)
            {
                var tasks = db.Tasks.Where(x => x.GameId != null && x.GameId == game.Id);
                if (tasks != null && tasks.Any())
                {
                    return tasks.ToList();
                }
            }
            return new List<GameTask>();
        }

        public static List<GameTask> GetAllTasks(long chatId)
        {
            var game = GameManager.GetActiveGameByChatId(chatId);
            if (game == null || game.Id == null) return new List<GameTask>();
            return GetAllTasks(game);
        }

    }
}
