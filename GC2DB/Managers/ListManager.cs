using GC2DB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GC2DB.Managers
{
    public static class ListManager
    {
        public static string? GetList(long chatId, bool sorted = false)
        {
            using var db = DBContext.Instance;
            var result = db.Lists
                .Where(x => x.ChatId == chatId)
                .Select(x => x.Text)?.ToList();
            if (result != null && result.Any())
            {
                if (sorted)
                {
                    result.Sort();
                }
                return String.Join("\r\n", result);
            }
            else
            {
                return null;
            }
        }

        public static void ClearList(long chatId)
        {
            using var db = DBContext.Instance;
            var entitiesToDelete = db.Lists.Where(x => x.ChatId == chatId)?.ToList();
            if (entitiesToDelete != null && entitiesToDelete.Any())
            {
                db.Lists.RemoveRange(entitiesToDelete);
                db.SaveChanges();
            }


        }

        public static void AddValue(long chatId, string value)
        {
            using var db = DBContext.Instance;
            db.Lists.Add(new ListItem(chatId, value));
            db.SaveChanges();
        }
    }
}
