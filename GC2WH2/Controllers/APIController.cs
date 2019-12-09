using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GC2;
using GC2DB;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GC2WH2.Controllers
{
    public class APIController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Index()
        {
            try
            {
                var req = Request.Body;
                var responsString = await new StreamReader(req).ReadToEndAsync(); //read request
                Update update = JsonConvert.DeserializeObject<Update>(responsString);

                ReceivedMessage receivedMessage;
                if (update.Type == UpdateType.Message || update.Type == UpdateType.EditedMessage)
                {
                    var message = update.Message;
                    if (message == null) return Ok();

                    receivedMessage = new ReceivedMessage()
                    {
                        Text = message.Text,
                        Image = message.Photo,
                        Coordinates = (message.Location == null) ? null : new Coordinates(message.Location),

                        ChatId = message.Chat.Id,
                        Id = message.MessageId,
                        PrivateChat = (message.Chat.Type == ChatType.Private),
                        IsCallback = false,
                        SenderId = message.From.Id,

                        ReplyTo = (message.ReplyToMessage == null) ? (int?)null : message.ReplyToMessage.MessageId,
                        ReplyToText = (message.ReplyToMessage == null) ? (string)null : message.ReplyToMessage.Text,
                        ReplyToBot = (message.ReplyToMessage == null) ? false : (message.ReplyToMessage.From.IsBot && message.ReplyToMessage.From.Id == BotReference.Bot.BotId),
                    };
                }
                else //Callback
                {
                    var callback = update.CallbackQuery;
                    if (callback == null) return Ok();
                    receivedMessage = new ReceivedMessage()
                    {
                        Text = callback.Data,
                        ChatId = callback.Message.Chat.Id,
                        Id = callback.Message.MessageId,
                        PrivateChat = (callback.Message.Chat.Type == ChatType.Private),
                        IsCallback = true,
                        SenderId = callback.From.Id,
                    };
                }

                receivedMessage.Normalise();

                if (update.Type == UpdateType.Message || update.Type == UpdateType.EditedMessage)
                {
                    var message = update.Message;
                    if (message.Photo != null)
                    {
                        if (message.Photo.Length > 0)
                        {
                            var obj = message.Photo.GetValue(message.Photo.Length - 1);
                            if (obj != null && obj is PhotoSize ps)
                            {
                                var imageId = ps.FileId;
                                using var ms = new MemoryStream();
                                receivedMessage.Image = await BotReference.Bot.GetInfoAndDownloadFileAsync(imageId, ms);
                                receivedMessage.Image = ms.ToArray();
                            }
                        }
                    }
                }

                var result = Processing.ProcessMessage(receivedMessage);
                if (result == null) return Ok();
                await result.Finish(BotReference.Bot, receivedMessage);
                return Ok();
            }
            catch(Exception ex)
            {
                var gex = new GCException(ex).GetForDb();
                using (var db = DBContext.Instance)
                {
                    db.Exceptions.Add(gex);
                    db.SaveChanges();
                }
                return Ok();
            }
        }


    }
}