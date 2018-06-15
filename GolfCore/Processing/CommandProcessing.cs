using GolfCoreDB;
using GolfCoreDB.Data;
using GolfCoreDB.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GolfCore.Processing
{
    public class CommandProcessing
    {
        public static ProcessingResult Process(string command, string parameters, long chatId)
        {
            switch (command.ToLower())
            {
                case "foo":
                    return new ProcessingResult("bar", chatId);

                case Constants.Commands.ShowSettings:
                    return ShowSettings(chatId);

                case Constants.Commands.UpdateSetting:
                    var values = GetParameters(parameters, 2);
                    return UpdateSetting(values, chatId);

                case "help":
                    return new ProcessingResult(Constants.Help, chatId);

                default:
                    return GameCommandProcessing.Process(command, GetParameters(parameters, null), chatId);
            }
        }



        private static ProcessingResult ShowSettings(long chatId)
        {
            using (var db = new DBContext())
            {
                var s = db.Settings.ToList();
                return new ProcessingResult("Data:\r\n" + String.Join("\r\n", s.Select<Setting, string>(x => x.Name + " = " + x.Value))
                    ,chatId);
            }
        }

        private static ProcessingResult UpdateSetting(List<string> values, long chatId)
        {
            using (var db = new DBContext())
            {
                var s = db.Settings.ToList();
                if (s.Where(x => x.Name == values[0]).Any())
                {
                    Setting setting = s.Where(x => x.Name == values[0]).First();
                    setting.Value = values[1];
                    db.SaveChanges();
                }
                else
                {
                    db.Settings.Add(new Setting()
                    {
                        Name = values[0],
                        Value = values[1],
                        ChatId = chatId
                    });
                    db.SaveChanges();
                }
                return null;
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
