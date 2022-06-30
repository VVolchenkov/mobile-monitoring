using System;
using Infrastructure.Enums;

namespace Web.Models
{
    public class DeviceDto
    {
        public string Name { get; set; }
        public Platform Os { get; set; }
        public string Version { get; set; }
        public DateTime LastUpdate { get; set; }
        public int Id { get; set; }
    }
}
