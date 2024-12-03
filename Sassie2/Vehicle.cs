using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sassie2
{
    class Vehicle
    {
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }

        public Vehicle(string year, string make, string model)
        {
            this.Year = year;
            this.Make = make;
            this.Model = model;
        }

    }
}
