using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace GolfCoreDB.Data
{
    public class Game
    {
        [Key]
        public string Id { get; set; }
        public GameType Type { get; set; }
        public List<GameParticipant> Participants { get; set; }
        public bool IsActive { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string LastTask { get; set; }
        public byte[] LastStatistics { get; set; }
        public int? LastStatisticsHash { get; set; }

        public string Title { get; set; }
        public string Href { get; set; }
        public string EnCxId { get; set; }

        
    }
}
