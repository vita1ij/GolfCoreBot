using GC2DB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GC2WH2.Models
{
    public class GameInfo
    {
        public string Guid { get; set; }
        public long Key { get; set; }
        public string Secret { get; set; }
        public Game Game { get; set; }
        public List<GameTask> Tasks { get; set; }
    }
}
