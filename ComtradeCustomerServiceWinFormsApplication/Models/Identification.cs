using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeCustomerServiceWinFormsApplication.Models
{
    class Identification
    {
        public Identification(string id, string name, string ssn) {
            Id = id;
            Name = name;
            SSN = ssn;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string SSN { get; set; }
    }
}
