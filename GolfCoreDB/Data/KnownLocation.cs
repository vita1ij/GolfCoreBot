using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace GolfCoreDB.Data
{
    public class KnownLocation
    {
        [Key]
        public int? Id { get; set; }
        public string Address { get; set; }
        public double Lon { get; set; }
        public double Lat { get; set; }
        public string Status { get; set; }

        public override bool Equals(object obj)
        {
            var data = (KnownLocation)obj;
            if (this.Id.HasValue && data.Id.HasValue) return (this.Id.Value == data.Id.Value);
            return (this.Lat == data.Lat && this.Lon == data.Lon);
        }
    }
}
