using System;

namespace Routine.Api.Models
{
    public class CompanyFullDto
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Country { get; set; }
        public String Industry { get; set; }
        public String Product { get; set; }
        public String Introduction { get; set; }
    }
}
