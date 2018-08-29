using GolfCoreDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GolfCoreDB.Managers
{
    public static class LocationsManager
    {
        //miles, kilometers, nautical miles... all that magic.
        private const double MagicNumber = 6370.6934856530580439461631130889; 

        public static void UpdateLocation(KnownLocation location)
        {
            using (var db = new DBContext())
            {
                KnownLocation existing = null;
                if (location.Id.HasValue)
                {
                    existing = db.Locations.Where(x => x.Id == location.Id.Value).First();
                }
                else
                {
                    existing = db.Locations.Where(x => x.Lon == location.Lon && x.Lat == location.Lat && x.Address == location.Address).FirstOrDefault();
                }

                if (existing == null)
                {
                    db.Locations.Add(location);
                }
                else
                {
                    if (existing.Status != location.Status)
                    {
                        existing.Status = location.Status;
                        db.Locations.Update(existing);
                    }
                }

                db.SaveChanges();
            }
        }

        public static KnownLocation CheckLocation(double lat, double lon, out double distance)
        {
            KnownLocation nearest = null;
            double minDistance;

            using (var db = new DBContext())
            {
                var locations = db.Locations.ToList();
                nearest = locations[0];
                minDistance = Distance(nearest.Lat, nearest.Lon, lat, lon);
                locations.RemoveAt(0);

                foreach (var loc in locations)
                {
                    var dist = Distance(loc.Lat, loc.Lon, lat, lon);
                    if (dist < minDistance)
                    {
                        nearest = loc;
                        minDistance = dist;
                    }
                }

                distance = minDistance;
                return nearest;
            }
        }

        public static double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);

            dist = dist * MagicNumber;

            return dist;
        }
    }
}
