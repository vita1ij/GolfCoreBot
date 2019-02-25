using GolfCoreDB;
using GolfCoreDB.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HtmlAgilityPack;

namespace GolfCore.Helpers
{
   // public class FBHelper
    //{
    //    public const string FB_EVENT_URL_FORMAT = "https://www.facebook.com/pg/{0}/events/?ref=page_internal";
    //    public const string FB_PAGES_SETTING = "FB_PAGES";
    //    public static List<string> ALLOWED_PAGES = new List<string>() { "igra.lv", "spele.lv", "Quest.lv"};

    //    public List<String> GetFollowingPages(long chatId)
    //    {
    //        throw new NotImplementedException();
    //        //var pages = SettingsManager.GetSetting(chatId, FB_PAGES_SETTING);
    //        //if (pages != null)
    //        //{
    //        //    return pages.Split("|").ToList();
    //        //}
    //        //return null;
    //    }

    //    public List<string> GetUpcommingEvents(long chatId)
    //    {
    //        List<string> result = new List<string>();

    //        //var pages = GetFollowingPages(chatId);
    //        var pages = ALLOWED_PAGES; // for testing purposes
    //        if (pages == null)
    //        {
    //            result.Add(Constants.Settings.EmptySetting(FB_PAGES_SETTING));
    //        }
    //        else
    //        {
    //            foreach(var page in pages)
    //            {
    //                var data = WebConnectHelper.MakePostWithCookies(String.Format(FB_EVENT_URL_FORMAT, page),null);
    //                HtmlDocument doc = new HtmlDocument();
    //                doc.LoadHtml(data);
    //                HtmlNode upcommingEvents = doc.GetElementbyId("upcoming_events_card");
    //                if (upcommingEvents.ChildNodes.Any())
    //                {
    //                    var events = upcommingEvents.SelectNodes("table");
    //                }

    //            }
                
    //        }
    //        return result;
    //    }


    //}
}
