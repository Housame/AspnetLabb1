using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Labb10.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public DateTime DateOfCreation { get; set; }
        public DateTime DateOfEdit{ get; set; }
    }
}
