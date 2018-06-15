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
    public class MinuteDaemon
    {
        public async Task RunAsync(TelegramBotClient bot)
        {
            using (var db = DBContext.Instance)
            {
                var games = db.Games.Where(x => x.IsActive && x.Participants.Any(y => y.MonitorUpdates)).Include("Participants");
                if (games == null || !games.Any()) return;
                foreach (var game in games)
                {
                    string newDate = DateTime.UtcNow.Date.AddMinutes(DateTime.Now.Minute).ToString();
                    if (game.LastTask != newDate)
                    {
                        game.LastTask = newDate;
                        await db.SaveChangesAsync();

                        if (game.Participants != null &&
                            game.Participants.Any(x => x.MonitorUpdates))
                        {
                            foreach (var participant in game.Participants.Where(x => x.MonitorUpdates))
                            {
                                string txt = participant.GetUpdates ?
                                    newDate :
                                    "1";
                                await bot.SendTextMessageAsync(participant.ChatId
                                    , txt);
                            }
                        }
                    }
                }
            }
            
        }
    }
}
