using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GC2DB.Data
{
    public class Task
    {
        [Key]
        [Required]
        public long Id { get; set; }
        public long? Number { get; set; }
        public string? Title { get; set; }
        public string Text { get; set; }
        public virtual Game Game { get; set; }

        public Task(Game game, string text)
        {
            this.Game = game;
            this.Text = text;
        }

        //public Task(long gameId, string text)
        //{
        //    this.Game = new Game() { Id = gameId };
        //    this.Text = text;
        //}

        /// <summary>
        /// Only for EF. Do not use it
        /// </summary>
        public Task()
        {
        }
    }
}
