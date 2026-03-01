using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootMobile
{
    internal class PlantPinData
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string ImagePath { get; set; } // Шлях до фото на пристрої
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
