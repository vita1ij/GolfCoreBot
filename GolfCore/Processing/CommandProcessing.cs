using GolfCoreDB;
using GolfCoreDB.Data;
using GolfCoreDB.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace GolfCore.Processing
{
    public class CommandProcessing
    {
        public static ProcessingResult Process(string command, string parameters, long chatId, int messageId)
        {
            switch (command.ToLower())
            {
                case "foo":
                    return new ProcessingResult("bar", chatId);

                case Constants.Commands.ShowSettings:
                    return ShowSettings(chatId);

                case Constants.Commands.UpdateSetting:
                    var values = GetParameters(parameters, 2);
                    SettingsManager.UpdateSetting(values[0], values[1], chatId);
                    return null;

                case Constants.Commands.Help:
                    return new ProcessingResult(Constants.Help, chatId);

                default:
                    return GameCommandProcessing.Process(command, GetParameters(parameters, null), chatId);
            }
        }

        public static ProcessingResult ShowSettings(long chatId)
        {
            var data = SettingsManager.GetSettings(chatId);
            return new ProcessingResult("Data:\r\n" + String.Join("\r\n", data.Select<Tuple<string,string>, string>(x => x.Item1 + " = " + x.Item2)), chatId);
        }


        #region Helpers
        public static List<String> GetParameters(string input, int? count)
        {
            var values = input.Split(' ').Where(x => x != "").ToList();
            if (count.HasValue && values.Count != count.Value)
            {
                throw new Exception(Constants.Exceptions.WrongParameterCount(count.Value));
            }
            return values;
        }
        #endregion
    }
}
