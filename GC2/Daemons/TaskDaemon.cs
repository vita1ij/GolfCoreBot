using GC2.Engines;
using GC2DB.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace GC2.Daemons
{
    public class TaskDaemon : Daemon
    {
        public new static double Interval = 5000;

        public override async Task Function(TelegramBotClient bot)
        {
            var players = GameManager.GetAllActivePlayersWithTaskMonitoring();
            if (players == null || !players.Any()) return;
            var games = players.Select(x => x.Game).Distinct();
            foreach (var game in games)
            {
                var engine = IGameEngine.Get(game);
                var newTask = engine.GetTask(out List<object> stuff);
                if (newTask == null) continue;
                var lastTask = (game.LastTaskId.HasValue) 
                    ? GameManager.GetTaskById(game.LastTaskId.Value)
                    : null;
                if (lastTask == null || !lastTask.Equals(newTask))
                {
                    newTask.Game = game;
                    GameManager.AddTask(newTask);

                    foreach(var player in players.Where(x => x.Game.Id == game.Id))
                    {
                        var result = new ProcessingResult()
                        {
                            ChatId = player.ChatId,
                            IsHtml = false,
                            Text = player.UpdateTaskInfo == GC2DB.Data.Player.PlayerUpdateStatusInfo.UpdateText ? newTask.Text : "UP!"
                        };
                        await result.Finish(bot);
                    }
                }
            }
        }
    }
}
