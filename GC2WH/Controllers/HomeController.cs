using GC2DB;
using GC2WH.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GC2WH.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Status()
        {
            var whInfo = await BotReference.Bot.GetWebhookInfoAsync();

            var sm = new StatusModel()
            {
                CanStartBot = true,
                CanStopBot = true,
                PendingUpdates = whInfo.PendingUpdateCount,
                LastErrorDate = whInfo.LastErrorDate,
                LastErrorMessage = whInfo.LastErrorMessage,
                MaxConnections = whInfo.MaxConnections,
                AllowedUpdates = String.Join(" / ", whInfo.AllowedUpdates)
            };
            
            sm.BotWebhookSet = !String.IsNullOrEmpty(whInfo?.Url);
            sm.BotLocationDbLastUpdated = new DateTime(1);

            //Tests
            sm.Tests.DBConnection = TestDbConnection();
            
            return View("Status",sm);
        }

        public async Task<IActionResult> StopBot()
        {
            await BotReference.Bot.DeleteWebhookAsync();
            return await Status();
        }

        public async Task<IActionResult> StartBot()
        {
            await BotReference.Bot.SetWebhookAsync(Program.Config["WEBHOOK_URL"]);
            return await Status();
        }

        //[route(".well-known/acme-challenge/")]
        //public iactionresult foo()
        //{
        //    return view("index");
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static string TestDbConnection()
        {
            try
            {
                using (var db = DBContext.Instance)
                {
                    var lists = db.Lists.ToList();
                }
                return "Ok";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
