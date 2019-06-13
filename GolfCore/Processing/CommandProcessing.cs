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
        public static ProcessingResult? Process(string command, string? parameters, long chatId)
        {
            switch (command.ToLower())
            {

                case Constants.Commands.CheckAddress:
                    return (parameters == null) ?
                        (ProcessingResult?)null :
                        LocationsHelper.ParseCoordinates(parameters, out string _, out _) ?
                            new ProcessingResult(LocationsHelper.GetAddress(parameters), chatId) :
                            new ProcessingResult(LocationsHelper.GetCoordinates(parameters,"Riga") ?? Constants.Text.NoResults, chatId, true, true);
                case Constants.Commands.HideKeyboard:
                    return new ProcessingResult("ok", chatId, new ReplyKeyboardRemove());
                case "starttalk":
                    return (parameters == null) ? null : ConversationsProcessing.StartConversation(parameters, chatId);
                case Constants.Commands.UpdateKnownLocationsDB:
                    return new ProcessingResult(LocationsHelper.UpdateDatabase(), chatId);
                case Constants.Commands.CheckLocation:
                    return (parameters == null) ? null : new ProcessingResult(LocationsHelper.CheckLocation(parameters) ?? Constants.Text.NoResults, chatId);
                case Constants.Commands.Help:
                    return new ProcessingResult(Constants.Help, chatId);
                case "dist":
                    if (parameters == null || String.IsNullOrWhiteSpace(parameters) || parameters.Split(' ',StringSplitOptions.RemoveEmptyEntries).Count() != 4) return null;
                    var p = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    return new ProcessingResult(LocationsHelper.CalculateDistance(p[0], p[1], p[2], p[3]).ToString(), chatId);
                case "alltasks":
                    var result = TasksManager.GetAllTasks(chatId);
                    if (result == null || !result.Any()) return null;
                    return new ProcessingResult(
                            String.Join(" \r\n\r\n ", result.Select(x => x.TaskText ?? ""))
                        , chatId);
                default:
                    return GameCommandProcessing.Process(command, GetParameters(parameters??"",null), chatId);
            }
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
