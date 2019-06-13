using System.Collections.Generic;
using GolfCoreDB.Data;
using GolfCoreDB.Managers;

namespace GolfCore.GameEngines
{
    public abstract class IEnCxEngine : IGameEngine
    {
        public string? GamesUrl;

        public abstract string GetFullGamesLink(string? type, string? status, string? zone);
        public abstract List<string> GetAllGameZones();
        public abstract List<Game> GetEnCxGames(string? type, string? status, string? zone, out List<(string, string)> pages);
    }
}
