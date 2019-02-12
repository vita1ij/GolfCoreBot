using GolfCore.Daemons;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Telegram.Bot;

namespace GC.StatidticsService
{
    class Program
    {
        static void Main(string[] args)
        {
            while (!Console.KeyAvailable)
            {
                try
                {
                    TaskDaemon taskDaemon = new TaskDaemon();
                    taskDaemon.RunAsync(Bot).Wait();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Config["ERROR_FOLDER"] + DateTime.Now.ToLongDateString() + ".log", ex.Message);
                }
            }
            Console.ReadLine();
        }

        public static IConfiguration Config
        {
            get
            {
                var c = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("config.json");
                return c.Build();
            }
        }

        public static readonly TelegramBotClient Bot = new TelegramBotClient(Config["API_KEY"].ToString());
    }
}
