﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GolfCore.Helpers
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
    }
}