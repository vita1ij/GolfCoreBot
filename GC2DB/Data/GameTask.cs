using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GC2DB.Data
{
    public class GameTask
    {
        [Key]
        [Required]
        public long Id { get; set; }
        public long? Number { get; set; }
        public string EnCxId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public virtual Game Game { get; set; }

        public GameTask(Game game, string text)
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
        public GameTask()
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GameTask)) return false;

            var task = obj as GameTask;

            if (!String.IsNullOrWhiteSpace(this.EnCxId)
                && !String.IsNullOrWhiteSpace(task.EnCxId)
                )
            {
                if (this.Number != task.Number) return false;

                return (this.Text == task.Text); //todo[vg]: Implement comparison based on percentage
            }

            return (this.Text == task.Text);
        }
    }
}
