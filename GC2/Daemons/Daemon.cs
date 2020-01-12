using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace GC2.Daemons
{
    public abstract class Daemon
    {
        public static bool CanRun = true;
        private static bool inProgress = false;
        public static double Interval = 10000;

        public abstract Task Function(TelegramBotClient bot);

        private async Task RealFunction(TelegramBotClient bot)
        {
            if (inProgress) return;
            if (!CanRun) return;
            inProgress = true;
            try
            {
                await Function(bot);
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

        public IObservable<Task> Create(TelegramBotClient bot)
        {
            return Observable.Create<Task>(
                observer =>
                {
                    var timer = new System.Timers.Timer
                    {
                        Interval = Interval
                    };
                    timer.Elapsed += (s, e) => observer.OnNext(RealFunction(bot));
                    timer.Start();
                    return timer;
                });
        }
    }
}
