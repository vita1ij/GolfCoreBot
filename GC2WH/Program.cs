using GC2;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace GC2WH
{
    public class Program
    {
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

        private static void Setup()
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

        public static void Main(string[] args)
        {
            Setup();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
