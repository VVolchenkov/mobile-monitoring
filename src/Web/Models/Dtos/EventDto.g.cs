using System;

namespace Web.Models
{
    public partial record EventDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public int DeviceId { get; set; }
    }
}
