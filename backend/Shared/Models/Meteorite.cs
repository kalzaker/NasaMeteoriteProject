using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    public class Meteorite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string MeteoriteId { get; set; }
        public string NasaId { get; set; }
        public string Name { get; set; }
        public string Nametype { get; set; }
        public string Recclass { get; set; }
        public double? Mass { get; set; }
        public string Fall { get; set; }
        public DateTime? Year { get; set; }
        public float? Reclat { get; set; }
        public float? Reclong { get; set; }
        public double? GeoLat { get; set; }
        public double? GeoLong { get; set; }
    }
}
