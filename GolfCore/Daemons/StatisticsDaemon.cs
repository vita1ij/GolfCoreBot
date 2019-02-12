using GolfCore.GameEngines;
using GolfCoreDB.Managers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace GolfCore.Daemons
{
    public class StatisticsDaemon
    {
        public async Task RunAsync(TelegramBotClient bot)
        {
            try
            {
                var games = GameManager.GetAllActiveWithStatsMonitoring();
                if (games == null || !games.Any()) return;
                foreach (var game in games)
                {
                    bool needUpdate = false;
                    if (game.Participants != null &&
                            game.Participants.Any(x => x.MonitorUpdates))
                    {
                        Image<Rgba32> newStat = null;
                        foreach (var participant in game.Participants.Where(x => x.MonitorUpdates))
                        {
                            IGameEngine engine = IGameEngine.Get(participant);

                            newStat = engine.GetStatistics();

                            if ((game.LastStatisticsHash.HasValue ? game.LastStatisticsHash.Value : (int)-1) != newStat.GetHashCode())
                            {
                                using (var sold = new MemoryStream(game.LastStatistics))
                                {
                                    Image<Rgba32> oldStat = Image.Load<Rgba32>(sold.ToArray());
                                    if (oldStat != newStat)
                                    {
                                        //Update
                                        needUpdate = true;

                                        using (var s = new MemoryStream())
                                        {
                                            newStat.SaveAsBmp(s);
                                            await bot.SendPhotoAsync(participant.ChatId, new Telegram.Bot.Types.InputFiles.InputOnlineFile(s));
                                        }

                                    }
                                }
                            }
                        }

                        if (needUpdate)
                        {
                            using (var s = new MemoryStream())
                            {
                                newStat.SaveAsBmp(s);
                                game.LastStatistics = s.ToArray();
                            }
                            game.LastStatisticsHash = newStat.GetHashCode();
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
