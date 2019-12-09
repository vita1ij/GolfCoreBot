using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GC2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace GC2WH2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            setup();
            CreateHostBuilder(args).Build().Run();
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
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void setup()
        {
            if (Config == null) throw new Exception("No config");
            if (Config["API_KEY"] == null) throw new Exception("No API key");
            if (Config["ERROR_FOLDER"] == null) throw new Exception("No Error folder selected");
            if (String.IsNullOrEmpty(Config["API_KEY"])) throw new Exception("Empty API key");
            try
            {
                BotReference.Bot = new TelegramBotClient(Config["API_KEY"].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Cannot initialise Bot: {0}---------------{1}", ex.Message, ex.InnerException));
            }
            try
            {
                var exreftask = BotReference.Bot.GetWebhookInfoAsync();
                exreftask.Wait();
                var exref = exreftask.Result;
                if (exref != null && !String.IsNullOrWhiteSpace(exref.Url))
                {
                    BotReference.Bot.DeleteWebhookAsync().Wait();
                }
                BotReference.Bot.SetWebhookAsync("https://bot.gagunovs.id.lv/API/", null, 0,
                    new List<UpdateType>()
                    {
                        UpdateType.Message,
                        UpdateType.EditedMessage,
                        UpdateType.CallbackQuery
                    });
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Cannot set up Bot: {0}---------------{1}", ex.Message, ex.InnerException));
            }

            try
            {
                //new TaskDaemon().Create(Bot).Subscribe();
            }
            catch (Exception ex)
            {
                Log.New(ex);
            }

            //Console.ReadLine();
            //Bot.StopReceiving();
        }
    }
}
