using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GC2WH.Models;

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
            var sm = new StatusModel()
            {
                CanStartBot = true,
                CanStopBot = true
            };
            var whInfo = await BotReference.Bot.GetWebhookInfoAsync();
            sm.BotWebhookSet = !String.IsNullOrEmpty(whInfo?.Url);
            sm.BotLocationDbLastUpdated = new DateTime(1);
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
    }
}
