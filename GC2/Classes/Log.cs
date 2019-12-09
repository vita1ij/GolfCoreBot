﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
//using System.Web.uHelpers;

namespace GC2
{
    public static class Log
    {
        private static IConfiguration Config
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
            if (String.IsNullOrWhiteSpace(Config["ERROR_FOLDER"]?.ToString()))
            {
                return;
            }
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
            if (String.IsNullOrWhiteSpace(Config["ERROR_FOLDER"]?.ToString()))
            {
                return;
            }
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
                JsonSerializer.Serialize(message),
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
