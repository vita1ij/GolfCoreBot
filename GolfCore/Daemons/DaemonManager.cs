using GolfCore.Processing;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace GolfCore.Daemons
{
    public static class DaemonManager
    {
        private static List<(IDaemon, string)> ActiveDaemons;

        public delegate Task Func(TelegramBotClient bot);

        public static IObservable<Task> Create(Func f, TelegramBotClient bot)
        {
            return Observable.Create<Task>(
                observer =>
                {
                    var timer = new System.Timers.Timer
                    {
                        Interval = 1000
                    };
                    timer.Elapsed += (s, e) => observer.OnNext(f(bot));
                    timer.Start();
                    return timer;
                });
        }
    }
}
