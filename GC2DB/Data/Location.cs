using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GC2DB.Data
{
    public class Location
    {
        [Key]
        [Required]
        public int? Id { get; set; }
        public string? Address { get; set; }
        public double Lon { get; set; }
        public double Lat { get; set; }
        public string? Status { get; set; } //todo[gc2]:enum


        public override bool Equals(object obj)
        {
            var data = (Location)obj;
            if (this.Id.HasValue && data.Id.HasValue) return (this.Id.Value == data.Id.Value);
            return (this.Lat == data.Lat && this.Lon == data.Lon);
        }
        public override int GetHashCode()
        {
            int? result = this.Id ?? (int)(Math.Round(Lat * 100) * 100000 + Math.Round(Lon * 100));
            return result.GetValueOrDefault();
        }
        public override string ToString()
        {
            return $"{Address} ({Lat}, {Lon})";
        }
    }
}
