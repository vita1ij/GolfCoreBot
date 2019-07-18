using System;
using System.Collections.Generic;
using System.Text;

namespace GC2.StaticData
{
    public static class EnCxGameList
    {
        private static TimeSpan UpdatePeriod = TimeSpan.FromMinutes(10);
        private static Dictionary<(string, string), (DateTime, List<(string, long)>)> _games = new Dictionary<(string, string), (DateTime, List<(string, long)>)>();


        public static List<String> Zones = new List<string>{
                "Real",
                "Points",
                "All"
            };

        public static List<(string,long)> Games(string zone, string status)
        {
            if (!_games.ContainsKey((zone,status))
                || _games[(zone,status)].Item1 + UpdatePeriod < DateTime.Now)
            {
                Update(zone, status);
            }
            if (!_games.ContainsKey((zone, status))) return new List<(string, long)>();
            return _games[(zone, status)].Item2;
        }

        public static void Update(string zone, string status)
        {
            //todo[gc2]
        }
    }
}
