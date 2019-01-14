using GolfCore.GameEngines;
using GolfCoreDB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace GolfCore.Daemons
{
    public class TaskDaemon
    {
        public async Task RunAsync(TelegramBotClient bot)
        {
            try
            {
                using (var db = DBContext.Instance)
                {
                    var games = db.Games.Where(x => x.IsActive && x.Participants.Any(y => y.MonitorUpdates)).Include("Participants");
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
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}
