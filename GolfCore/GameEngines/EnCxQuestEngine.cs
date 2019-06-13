using GolfCore.Helpers;
using GolfCoreDB.Data;
using GolfCoreDB.Managers;
using HtmlAgilityPack;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GolfCore.GameEngines
{
    public class EnCxQuestEngine : EnCxEngine
    {
        public const string HTMLPATH_ACTIVE_GAMES = "boxCenterActiveGames";
        public const string HTMLPATH_ACTIVE_GAME = "div.boxGameInfo table.gameInfo table tr::first td table ";
        

        public EnCxQuestEngine(long chatId)
            :base(chatId, "http://quest.en.cx/")
        {

        }

        public override Image<Rgba32>? GetStatistics()
        {
            throw new NotImplementedException();
        }

    }
}
