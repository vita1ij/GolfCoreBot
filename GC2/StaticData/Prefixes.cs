using GC2DB.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace GC2.StaticData
{
    public static class Prefixes
    {
        /// <summary>
        /// null if not initialised
        /// empty if no prefix set
        /// prefix if set
        /// </summary>
        private static Dictionary<long, string?> values = new Dictionary<long, string?>();

        /// <summary>
        /// returns null if prefix not set
        /// returns prefix otherwise
        /// </summary>
        /// <param name="chatId">Chat Id</param>
        /// <returns></returns>
        public static string? Get(long chatId)
        {
            if (!values.ContainsKey(chatId))
            {
                var activeGame = GameManager.GetActiveGameByChatId(chatId);
                values.Add(chatId, String.IsNullOrWhiteSpace(activeGame?.Prefix) ? "" : activeGame.Prefix);
            }
            var result = values[chatId];
            return String.IsNullOrEmpty(result) ? null : result;
        }
    }
}
