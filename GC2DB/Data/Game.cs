using GC2DB.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GC2DB.Data
{
    public class Game
    {
        [Key]
        [Required]
        public long Id { get; set; }
        public string Guid { get; set; }
        public GameType Type { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public long? LastTaskId { get; set; }
        public string Title { get; set; }
        public string Href { get; set; }
        public string EnCxId { get; set; }
        public string EnCxType { get; set; }
        public string Prefix { get; set; }
        public long? Radius { get; set; }
        public string CenterCoordinates { get; set; }
        public bool isActive { get; set; } = true;

        public Game()
        {
            this.Guid = System.Guid.NewGuid().ToString().Substring(0, 6);
        }
    }
}
