using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using GolfCoreDB.Managers;
using HtmlAgilityPack;
using GolfCore.Helpers;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GolfCore.GameEngines
{
    public class IgraLvGameEngine : IGameEngine
    {
        public IgraLvGameEngine(long chatId)
        {
            GameManager.GetAuthForActiveGame(chatId, out string? login, out string? pass);
            this.LoginPostData = $"login={login??""}&password={pass??""}";
            this.LoginUrl = "http://igra.lv/igra.php?s=login";
            this.TaskUrl = "http://www.igra.lv/igra.php";
            base.StatisticsUrl = "http://igra.lv/img_level_times.php";
            this.Login();
        }

        public override bool EnterCode(string code)
        {
            throw new NotImplementedException();
        }

        public override Image<Rgba32>? GetStatistics()
        {
            if (StatisticsUrl == null) return null;
            var response = WebConnectHelper.MakePostRaw(StatisticsUrl, "");
            if (response == null) return null;
            Stream receiveStream = response.GetResponseStream();

            Image<Rgba32> image = Image.Load(receiveStream);

            return image;
        }

        public override string? GetTask()
        {
            try
            {
                if (TaskUrl == null) return null;
                if (ConnectionCookie == null)
                {
                    if (LoginUrl == null || LoginPostData == null) return null;
                    ConnectionCookie = WebConnectHelper.MakePost4Cookies(LoginUrl, LoginPostData);
                    if (ConnectionCookie == null) return null;
                }
                else
                {
                    //fix    Expires: {1/1/0001 12:00:00 AM}
                    ConnectionCookie["agt_session"].Expires = DateTime.MinValue;
                }
                var data = WebConnectHelper.MakePostWithCookies(TaskUrl, ConnectionCookie);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(data);
                string taskContent = (doc.GetElementbyId("general-puzzle") ?? doc.GetElementbyId("general")).InnerText;

                return taskContent;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public override bool IsLoginPage(string data)
        {
            return false;
        }
         
        //new public bool Login()
        //{

        //}
    }
}
