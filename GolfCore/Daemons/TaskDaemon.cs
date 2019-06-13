using GolfCore.GameEngines;
using GolfCoreDB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using GolfCoreDB.Managers;
using GolfCore.Helpers;
using GolfCoreDB.Data;

namespace GolfCore.Daemons
{
    public class TaskDaemon
    {
        public static bool inProgress = false;

        public static async Task Function(TelegramBotClient bot)
        {
            if (inProgress) return;
            inProgress = true;
            try
            {
                var games = GameManager.GetAllActiveWithMonitoring();
                if (games == null || !games.Any()) return;
                foreach (var game in games)
                {
                    if (game.Participants != null &&
                            game.Participants.Any(x => x.MonitorUpdates))
                    {
                        string? newTask = null;
                        foreach (var participant in game.Participants.Where(x => x.MonitorUpdates))
                        {
                            IGameEngine engine = IGameEngine.Get(participant);

                            newTask = engine.GetTask();

                            if (game.LastTask != newTask)
                            {
                                string txt = participant.GetUpdates ?
                                "A wild Task Appeared... \r\n" + newTask :
                                "A wild Task Appeared...";
                                await bot.SendTextMessageAsync(participant.ChatId
                                    , txt);
                            }
                        }

                        if (newTask != null && game.LastTask != newTask)
                        {
                            game.LastTask = newTask;
                            var gameTask = new GameTask(game.Id, newTask);
                            TasksManager.AddNewTask(gameTask);
                            GameManager.UpdateGame(game);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.New(ex);
            }
            finally
            {
                inProgress = false;
            }
        }

    }
}
