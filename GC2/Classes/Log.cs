using GC2DB.Data;
using GC2DB.Managers;
using Microsoft.Extensions.Configuration;
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
        public static void New(string s)
        {
            var e = new Error(text: s);
            CommonManager.LogError(e);
        }

        public static void New(Exception ex, ReceivedMessage? message = null)
        {
            var e = new Error(ex, chat:message?.ChatId, sender:message?.SenderId);
            CommonManager.LogError(e);
        }

    }
}
