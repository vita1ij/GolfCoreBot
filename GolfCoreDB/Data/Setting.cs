using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GolfCoreDB.Data
{
    public class Setting
    {
        [Key]
        public string Name { get; set; }
        public string Value { get; set; }
        public long ChatId { get; set; }
        public string GameId { get; set; }
    }
}
