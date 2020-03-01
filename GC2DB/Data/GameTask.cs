using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GC2DB.Data
{
    public class GameTask
    {
        [Key]
        public long? Id { get; set; }
        public long? Number { get; set; }
        public string? EnCxId { get; set; }
        public string? Title { get; set; }
        public string Text { get; set; }
        public long? GameId { get; set; }
        public string? FullHtmlMain { get; set; }
        public string? HtmlLeft { get; set; }
        public string? HtmlAdditional { get; set; }

        public GameTask(string text)
        {
            this.Text = text;
        }

        public GameTask(Game game, string text)
        {
            this.GameId = game.Id;
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
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public GameTask()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is GameTask task)) return false;
            if (task == null) return false;

            if (!String.IsNullOrWhiteSpace(this.EnCxId)
                && !String.IsNullOrWhiteSpace(task.EnCxId)
                )
            {
                if (this.Number != task.Number) return false;

                if (this.Text == null && task.Text != null) return true;
                if (task.Text == null) return false;
                if (Math.Abs((this?.Text?.Length ?? 0) - task.Text.Length) > 4) return false;
                var diff = 0;
                for(int i = 0; i < Math.Min(this?.Text?.Length ?? 0 , task.Text.Length); i++)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    if (Text[i] != task.Text[i]) diff++;
#pragma warning restore CS8602
                }
                return (diff < 4);
            }

            return (this.Text == task.Text);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
