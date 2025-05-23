using Shared.Misc;

namespace Shared.DTOs
{
    public class MeteoriteDto
    {
        [RequiredNotEmpty]
        public string Id { get; set; }
        [RequiredNotEmpty]
        public string Name { get; set; }
        public string Nametype { get; set; }
        public string Recclass { get; set; }
        public string Mass { get; set; }
        public string Fall { get; set; }
        public string Year { get; set; }
        public string Reclat { get; set; }
        public string Reclong { get; set; }
        public Geolocation Geolocation { get; set; }
    }

    public class Geolocation
    {
        public string Type { get; set; }
        public List<double> Coordinates { get; set; }
    }
}
