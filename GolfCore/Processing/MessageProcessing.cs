using GolfCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GolfCore.Processing
{
    public class MessageProcessing
    {
        public static ProcessingResult Process(string message, long chatId, int messageId, bool processRawText = true, bool isPrivate = false) //TODO[vg]: later change default processRawText value to false
        {
            if (String.IsNullOrEmpty(message)) return null;

            switch (message[0])
            {
                //Commands
                case '/':
                    var command = message.TrimStart('/').Split(' ')[0].Split('_')[0];
                    var param = message.Substring(command.Length + 1);
                    try
                    {
                        return CommandProcessing.Process(command, param.TrimStart('_'), chatId, messageId, isPrivate);
                    }
                    catch (Exception ex)
                    {
                        return new ProcessingResult(ex.Message, chatId);
                    }
                case '#':
                    //TODO[vg]: implement Lists
                    return null;
                default:
                    if (processRawText)
                    {
                        return ProcessText(message, chatId);
                    }
                    return null;
            }
        }

        public static ProcessingResult ProcessText(string message, long chatId)
        {
            ProcessingResult result = null;
            result = TryCoordinates(message, chatId);
            return result;
        }

        public static ProcessingResult TryCoordinates(string message, long chatId)
        {
            if (LocationsHelper.ParseCoordinates(message, out string lat, out string lon))
            { 
                return new ProcessingResult(
                    String.Format(Constants.CoordinatesString, lat, lon),
                    chatId,
                    true,
                    true
                    );
            }
            else
            {
                return null;
            }
        }
    }
}
