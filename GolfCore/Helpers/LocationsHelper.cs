﻿using Newtonsoft.Json;
using System;
using CsvHelper;
using System.IO;
using GolfCoreDB.Data;
using GolfCoreDB.Managers;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Nominatim.API;
using Nominatim.API.Geocoders;

namespace GolfCore.Helpers
{
    public static class LocationsHelper
    {
        public const string GRAUSTI_URL  = "http://grausti.riga.lv/";
        public const string GRAUSTI_URL2 = "http://grausti.riga.lv/ajax/module:constructions/";
        private const string GRAUSTI_POST = "owner=&region=&page=1&sortfield=&sortorder=&status=&filter=1&csv=1&action=getConstructions";
        private const string PbfPath = @"C:\vg\git\golfcorebot\GolfCore\Helpers\latvia-latest.osm.pbf";

        public static void UpdateDatabase()
        {

            var json = WebConnectHelper.MakePost(GRAUSTI_URL2, GRAUSTI_POST, "application/json, text/javascript");
            try
            {
                dynamic stuff = JsonConvert.DeserializeObject(json);
                string fileName = stuff.file.ToString();
                var csvData = WebConnectHelper.MakePost(GRAUSTI_URL + fileName, null);

                List<KnownLocation> locations = new List<KnownLocation>();
                using (TextReader r = new StringReader(csvData))
                {
                    var csv = new CsvReader(r);
                    while (csv.Read())
                    {
                        var row = csv.GetRecord<dynamic>();
                        var loc = new KnownLocation()
                        {
                            Address = row.Adrese,
                            Status = row.Buves_status
                        };
                        string cord = row.Koordinates;
                        //"56.945937, 24.106495"
                        if (!string.IsNullOrWhiteSpace(cord) && cord.Contains(","))
                        {
                            loc.Lat = double.Parse(cord.Split(",", StringSplitOptions.None)[0].Trim());
                            loc.Lon = double.Parse(cord.Split(",", StringSplitOptions.None)[1].Trim());
                        }

                        LocationsManager.UpdateLocation(loc);
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch(Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                //who cares
            }
        }

        public static string CheckLocation(string locationInput)
        {
            if (ParseCoordinates(locationInput, out string slat, out string slon)
                && double.TryParse(slat, out double lat)
                && double.TryParse(slon, out double lon)
                )
            {
                var location = LocationsManager.CheckLocation(lat, lon, out double distance);

                return $"Nearest location is in {distance}km. - {location.Address} ({lat}, {lon})";
            }
            return null;
        }

        public static bool ParseCoordinates(string input, out string lat, out string lon)
        {
            var match = Regex.Match(input, @"^[0-9., бю]+$");
            if (match.Success)
            {
                input = input.Replace('б', ',');
                input = input.Replace('ю', '.');

                input = input.Trim();
                if (input.Contains(' ') && input.Split(' ').Count() == 2)
                {
                    lat = input.Split(' ')[0];
                    lon = input.Split(' ')[1];
                }
                else if (input.Contains(',') && input.Split(',').Count() == 2)
                {
                    lat = input.Split(',')[0];
                    lon = input.Split(',')[1];
                }
                else if (input.Contains('.') && input.Split('.').Count() == 2)
                {
                    lat = input.Split('.')[0];
                    lon = input.Split('.')[1];
                }
                else
                {
                    input = input.Replace(",", "").Replace(".", "").Replace(" ", "");
                    lat = input.Substring(0, input.Length / 2);
                    lon = input.Substring(input.Length / 2);
                }

                lat = lat.Trim('.').Trim(',');
                lon = lon.Trim('.').Trim(',');

                lat = lat.Replace(',', '.');
                lon = lon.Replace(',', '.');

                if (!lat.Contains(".") && lat.Length > 1) lat = lat[0].ToString() + lat[1].ToString() + "." + lat.Substring(2);
                if (!lon.Contains(".") && lon.Length > 1) lon = lon[0].ToString() + lon[1].ToString() + "." + lon.Substring(2);

                if (!(lat.Length > 3 && lon.Length > 3))
                {
                    lat = lon = null;
                    return false;
                }
                return true;
            }
            else
            {
                lat = lon = null;
                return false;
            }
        }
        public static bool ParseCoordinates(string input, out double lat, out double lon)
        {
            string slat, slon;
            if (ParseCoordinates(input, out slat, out slon))
            {
                lat = double.Parse(slat);
                lon = double.Parse(slon);
                return true;
            }
            else
            {
                lat = lon = 0;
                return false;
            }
        }

        public static string GetCoordinates(string query, string city = "")
        {
            var foo = new Nominatim.API.Models.ForwardGeocodeRequest();
            foo.Country = "Latvia";
            foo.City = city;
            foo.queryString = query;
            var c = new  Nominatim.API.Geocoders.ForwardGeocoder();
            var r = c.Geocode(foo);
            //r.Result[0].
            var res = r.GetAwaiter().GetResult();
            string result = "";
            if (res.Count() == 0) return null;
            foreach(var rr in res)
            {
                result += String.Format(Constants.CoordinatesStringWithName, rr.Latitude, rr.Longitude,rr.DisplayName);
            }
            return result;
        }

        public static string GetAddress(string query)
        {
            double lat;
            double lon;
            if (ParseCoordinates(query, out lat, out lon))
            {
                var foo = new Nominatim.API.Models.ReverseGeocodeRequest();
                foo.Latitude = lat;
                foo.Longitude = lon;
                var c = new ReverseGeocoder();
                var r = c.ReverseGeocode(foo);
                var res = r.GetAwaiter().GetResult();
                var result =  res.Address.Country + ", " +  (res.Address.City ?? res.Address.Town) + ", " +  res.Address.Road + " " +  res.Address.HouseNumber ;
                return result;
            }
            else
            {
                return "";
            }
        }
    }
}
