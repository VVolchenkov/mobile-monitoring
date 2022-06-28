using System;

namespace Web.Models
{
    public partial record EventDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}