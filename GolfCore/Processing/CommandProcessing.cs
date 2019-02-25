using GolfCore.Helpers;
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
        public static ProcessingResult Process(string command, string parameters, long chatId, int messageId, bool isPrivate)
        {
            switch (command.ToLower())
            {
                case "a":
                    double lon, lat;
                    if(LocationsHelper.ParseCoordinates(parameters, out lat, out lon))
                    {
                        return new ProcessingResult(LocationsHelper.GetAddress(parameters), chatId);
                    }
                    else
                    {
                        return new ProcessingResult(LocationsHelper.GetCoordinates(parameters), chatId, true, true);
                    }
                case "aa":
                    
                case "hidekeyboard":
                    return new ProcessingResult("ok", chatId, new ReplyKeyboardRemove());
                case "sendprivate":
                    return new ProcessingResult("ok", -1);
                case "starttalk":
                    return ConversationsProcessing.StartConversation(GetParameters(parameters, 1)[0], chatId);
                case "updatedb":
                    LocationsHelper.UpdateDatabase();
                    break;

                case Constants.Commands.CheckLocation:
                    //return new ProcessingResult(LocationsManager.Distance(56.9263798, 24.0846039, 56.9636838, 24.2346644).ToString(), chatId);
                    return new ProcessingResult(LocationsHelper.CheckLocation(parameters), chatId);

                case Constants.Commands.Help:
                    return new ProcessingResult(Constants.Help, chatId);

                default:
                    return GameCommandProcessing.Process(command, GetParameters(parameters, null), chatId, isPrivate);
            }
            return null;
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
