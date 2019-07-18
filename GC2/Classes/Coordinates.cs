using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GC2
{
    public class Coordinates
    {
        private float latitude;
        private float longitude;

        public Coordinates(float lat, float lon)
        {
            latitude = lat;
            longitude = lon;
        }
        public Coordinates(Telegram.Bot.Types.Location loc)
        {
            latitude = loc.Latitude;
            longitude = loc.Longitude;
        }

        public float fLat
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value;
            }
        }
        public float fLon
        {
            get
            {
                return longitude;
            }
            set
            {
                longitude = value;
            }
        }
        public string Lat
        {
            get
            {
                return latitude.ToString();
            }
        }
        public string Lon
        {
            get
            {
                return longitude.ToString();
            }
        }

        public string? Address;//todo[gc2]

        public static Coordinates? ParseCoordinates(string? input)
        {
            if (String.IsNullOrEmpty(input)) return null;
            try
            {
                var parts = input
                            .Trim()
                            .Split(new char[] { '.', ',', ' ', 'б', 'ю' }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
                if (parts.Count == 2 || parts.Count == 4)
                {
                    if (parts.All(x => long.TryParse(x, out var _)))
                    {
                        if (parts.Count == 2)
                        {
                            if (parts[0].Length < 2 || parts[1].Length < 3) return null;
                            parts = new List<string>()
                        {
                            parts[0].Substring(0,2)
                            ,parts[0].Substring(2)
                            ,parts[1].Substring(0, 2)
                            ,parts[1].Substring(2)
                        };
                        }
                        return new Coordinates(float.Parse($"{parts[0]}{CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator}{parts[1]}")
                            , float.Parse($"{parts[2]}{CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator}{parts[3]}"));
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.New(ex);
                return null;
            }
        }

        public override string ToString()
        {
            return $"{Lat}, {Lon}";
            //if (String.IsNullOrEmpty(Address))
            //{
            //    return String.Format(Constants.Replies.LOCATION_FORMAT, this.latitude, this.longitude);
            //}
            //else
            //{
            //    return String.Format(Constants.Replies.LOCATION_WITH_NAME_FORMAT, this.latitude, this.longitude, this.Address);
            //}
            //
        }
    }
}
