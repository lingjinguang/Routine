using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.Entities
{
    public class Company
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Introduction { get; set; }
        public String Country { get; set; }
        public String Industry { get; set; }
        public String Product { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
