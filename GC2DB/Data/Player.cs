using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GC2DB.Data
{
    public class Player
    {
        [Key]
        [Required]
        public long Id { get; set; }
        public virtual Game Game { get; set; }
        public long ChatId { get; set; }
        public bool isActive { get; set; }
        public int UpdateTaskInfo { get; set; } = 0; //0/1/2
        public int UpdateStatisticsInfo { get; set; } = 0;

        public Player(Game game, long chatId)
        {
            this.Game = game;
            this.ChatId = chatId;
        }

        //public Player(long gameId, long chatId)
        //{
        //    this.Game = new Game() { Id = gameId };
        //    this.ChatId = ChatId;
        //}

        /// <summary>
        /// only for EF. Do not use it
        /// </summary>
        public Player()
        {
        }
    }
}
