using GolfCoreDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GolfCoreDB.Managers
{
    public class SettingsManager
    {
        public static List<Tuple<string,string>> GetSettings(long chatId)
        {
            using (var db = new DBContext())
            {
                var s = db.Settings.ToList();
                return s.ConvertAll<Tuple<string, string>>(x => new Tuple<string,string>(x.Name, x.Value));
            }
        }

        public static void UpdateSetting(string name, string value, long chatId)
        {
            using (var db = new DBContext())
            {
                var s = db.Settings.ToList();
                if (s.Where(x => x.Name == name).Any())
                {
                    Setting setting = s.Where(x => x.Name == name).First();
                    setting.Value = value;
                    db.SaveChanges();
                }
                else
                {
                    db.Settings.Add(new Setting()
                    {
                        Name = name,
                        Value = value,
                        ChatId = chatId
                    });
                    db.SaveChanges();
                }
            }
        }
    }
}
