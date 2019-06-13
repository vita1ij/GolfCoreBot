using System;
using System.Collections.Generic;
using System.Text;

namespace GolfCoreDB.Data
{
    public class GameTask
    {
        public string Id { get; set; }
        public string GameId { get; set; }
        public string? TaskText { get; set; }

        public GameTask(string gameId, string taskText)
        {
            this.Id = Guid.NewGuid().ToString().Replace("-", "");
            this.GameId = gameId;
            this.TaskText = taskText;
        }

        public GameTask()
        {
            this.Id = Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
