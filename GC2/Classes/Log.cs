using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Helpers;

namespace GC2
{
    public static class Log
    {
        public static IConfiguration Config
        {
            get
            {
                string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
                return new ConfigurationBuilder()
                    .SetBasePath(projectPath)
                    .AddJsonFile("config.json")
                    .Build();
            }
        }

        public static void New(Exception ex)
        {
            var folder = Config["ERROR_FOLDER"].ToString().TrimEnd('/').TrimEnd('\\');
            if (!Directory.Exists($"{folder}/{DateTime.Today.ToString("yyyyMMdd")}"))
            {
                Directory.CreateDirectory($"{folder}/{DateTime.Today.ToString("yyyyMMdd")}");
            }
            File.AppendAllLines($"{folder}/{DateTime.Today.ToString("yyyyMMdd")}/{DateTime.Now.ToString("hh-mm-ss")}.log", new[] {
                DateTime.Now.ToLongTimeString(),
                "",
                "Message",
                ex.Message,
                "",
                "Stack",
                ex.StackTrace ?? "",
                "",
                "Inner message",
                ex.InnerException?.Message ?? "",
                "",
                "Stack",
                ex.InnerException?.StackTrace ?? ""
            });
        }

        public static void New(GCException ex, ReceivedMessage message)
        {
            var folder = Config["ERROR_FOLDER"].ToString().TrimEnd('/').TrimEnd('\\');
            if (!Directory.Exists($"{folder}/{DateTime.Today.ToString("yyyyMMdd")}"))
            {
                Directory.CreateDirectory($"{folder}/{DateTime.Today.ToString("yyyyMMdd")}");
            }
            File.AppendAllLines($"{folder}/{DateTime.Today.ToString("yyyyMMdd")}/{DateTime.Now.ToString("hh-mm-ss")}.log", new[] {
                DateTime.Now.ToLongTimeString(),
                "",
                "ReceivedMessage",
                ex.Message,
                "",
                "Message",
                Json.Encode(message),
                "",
                "Code",
                ex.Code,
                "",
                "Stack",
                ex.StackTrace ?? "",
            });
        }
    }
}
