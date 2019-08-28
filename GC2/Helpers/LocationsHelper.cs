using Newtonsoft.Json;
using Nominatim.API.Geocoders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GC2.Helpers
{
    public static class LocationsHelper
    {
        public const string GRAUSTI_URL = "http://grausti.riga.lv/";
        public const string GRAUSTI_URL2 = "http://grausti.riga.lv/ajax/module:constructions/";
        private const string GRAUSTI_POST = "owner=&region=&page=1&sortfield=&sortorder=&status=&filter=1&csv=1&action=getConstructions";

        //public static string UpdateDatabase()
        //{

        //    var json = WebConnectHelper.MakePost(GRAUSTI_URL2, GRAUSTI_POST, "application/json, text/javascript");
        //    try
        //    {
        //        dynamic stuff = JsonConvert.DeserializeObject(json);
        //        string fileName = stuff.file.ToString();
        //        var csvData = WebConnectHelper.MakePost(GRAUSTI_URL + fileName, null);

        //        List<KnownLocation> locations = new List<KnownLocation>();
        //        using (TextReader r = new StringReader(csvData))
        //        {
        //            var csv = new CsvReader(r);
        //            while (csv.Read())
        //            {
        //                var row = csv.GetRecord<dynamic>();
        //                string cord = row.Koordinates;
        //                //"56.945937, 24.106495"
        //                double lat, lon;
        //                if (!string.IsNullOrWhiteSpace(cord) && cord.Contains(","))
        //                {
        //                    lat = double.Parse(cord.Split(",", StringSplitOptions.None)[0].Trim());
        //                    lon = double.Parse(cord.Split(",", StringSplitOptions.None)[1].Trim());

        //                    var loc = new KnownLocation(row.Adrese, lat, lon)
        //                    {
        //                        Status = row.Buves_status
        //                    };
        //                    LocationsManager.UpdateLocation(loc);
        //                }

        //            }
        //        }
        //        return "Complete";
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.New(ex);
        //        return "Error";
        //    }
        //}

        //public static string CheckLocation(string locationInput)
        //{
        //    if (ParseCoordinates(locationInput, out string slat, out string slon)
        //        && double.TryParse(slat, out double lat)
        //        && double.TryParse(slon, out double lon)
        //        )
        //    {
        //        var location = LocationsManager.CheckLocation(lat, lon, out double distance);
        //        if (location == null) return null;

        //        return $"Nearest location is in {distance}km. - {location.Address} ({lat}, {lon})";
        //    }
        //    return null;
        //}


        public static string GetCoordinates(string query, string city = "")
        {
            if (String.IsNullOrWhiteSpace(query)) return null;
            var foo = new Nominatim.API.Models.ForwardGeocodeRequest
            {
                Country = "Latvia",
                City = city,
                queryString = query
            };
            var c = new Nominatim.API.Geocoders.ForwardGeocoder();
            var r = c.Geocode(foo);
            //r.Result[0].
            var res = r.GetAwaiter().GetResult();
            string result = "";
            if (res.Count() == 0) return null;
            foreach (var rr in res)
            {
                result += String.Format(Constants.Replies.LOCATION_WITH_NAME_FORMAT, rr.Latitude, rr.Longitude, rr.DisplayName);
            }
            return result;
        }

        public static string GetAddress(Coordinates loc)
        {
            if (loc != null)
            {
                var foo = new Nominatim.API.Models.ReverseGeocodeRequest
                {
                    Latitude = loc.fLat,
                    Longitude = loc.fLon
                };
                var c = new ReverseGeocoder();
                var r = c.ReverseGeocode(foo);
                var res = r.GetAwaiter().GetResult();
                var result = res.Address.Country + ", " + (res.Address.City ?? res.Address.Town) + ", " + res.Address.Road + " " + res.Address.HouseNumber;
                return result;
            }
            else
            {
                return "";
            }
        }

        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double CalculateDistance(string _lat1, string _lon1, string _lat2, string _lon2)
        {
            if (double.TryParse(_lat1.Trim('.', ',', ' ').Replace('.', ',').Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator), out var lat1)
                && double.TryParse(_lat2.Trim('.', ',', ' ').Replace('.', ',').Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator), out var lat2)
                && double.TryParse(_lon1.Trim('.', ',', ' ').Replace('.', ',').Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator), out var lon1)
                && double.TryParse(_lon2.Trim('.', ',', ' ').Replace('.', ',').Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator), out var lon2))
                return CalculateDistance(lat1, lon1, lat2, lon2);
            else return 0;
        }

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var earthRadiusKm = 6371;

            var dLat = DegreeToRadian(lat2 - lat1);
            var dLon = DegreeToRadian(lon2 - lon1);

            lat1 = DegreeToRadian(lat1);
            lat2 = DegreeToRadian(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadiusKm * c;
        }
    }

}
