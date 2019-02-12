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

namespace GolfCore.Daemons
{
    public class TaskDaemon
    {
        public async Task RunAsync(TelegramBotClient bot)
        {
            try
            {
                var games = GameManager.GetAllActiveWithMonitoring();
                if (games == null || !games.Any()) return;
                foreach (var game in games)
                {
                    if (game.Participants != null &&
                            game.Participants.Any(x => x.MonitorUpdates))
                    {
                        string newTask = null;
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

                        if (game.LastTask != newTask)
                        {
                            game.LastTask = newTask;
                            GameManager.UpdateGame(game);
                        }
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch(Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {

            }
        }
    }
}
