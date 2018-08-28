using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GolfCore.Processing
{
    public class MessageProcessing
    {
        public static ProcessingResult Process(string message, long chatId, int messageId, bool processRawText = true) //TODO[vg]: later change default processRawText value to false
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
                        return CommandProcessing.Process(command, param.TrimStart('_'), chatId, messageId);
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
            //try first
            ProcessingResult result = null;
            result = TryCoordinates(message, chatId);
            if (result != null)
            {
                return result;
            }
            return null;
        }

        public static ProcessingResult TryCoordinates(string message, long chatId)
        {
            var match = Regex.Match(message, @"^[0-9., бю]+$");
            if (match.Success)
            {
                message = message.Replace('б', ',');
                message = message.Replace('ю', '.');
                string lat, lon;

                message = message.Trim();
                if (message.Contains(' ') && message.Split(' ').Count() == 2)
                {
                    lat = message.Split(' ')[0];
                    lon = message.Split(' ')[1];
                }
                else if (message.Contains(',') && message.Split(',').Count() == 2)
                {
                    lat = message.Split(',')[0];
                    lon = message.Split(',')[1];
                }
                else if (message.Contains('.') && message.Split('.').Count() == 2)
                {
                    lat = message.Split('.')[0];
                    lon = message.Split('.')[1];
                }
                else
                {
                    message = message.Replace(",", "").Replace(".", "").Replace(" ", "");
                    lat = message.Substring(0, message.Length / 2);
                    lon = message.Substring(message.Length / 2);
                }

                lat = lat.Trim('.').Trim(',');
                lon = lon.Trim('.').Trim(',');

                lat = lat.Replace(',', '.');
                lon = lon.Replace(',', '.');

                if (!lat.Contains(".") && lat.Length > 1) lat = lat[0].ToString() + lat[1].ToString() + "." + lat.Substring(2);
                if (!lon.Contains(".") && lon.Length > 1) lon = lon[0].ToString() + lon[1].ToString() + "." + lon.Substring(2);

                if (!(lat.Length > 3 && lon.Length > 3))
                {
                    return null;
                }

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
