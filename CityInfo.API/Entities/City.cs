using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CityInfo.API.Models;

namespace CityInfo.API.Entities
{
    public class City
    {
        [Key]                               // Explicitly say which is the key field.
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }         // Naming it Id automatically makes it a primary key. This is convention based again. Using attributes makes it explicit

        // Add the attributes here (at the lowest level of object), to ensure data integrity
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        public int NumberOfPointsOfInterest => PointsOfInterest.Count;

        public ICollection<PointOfInterest> PointsOfInterest { get; set; } =
            new List<PointOfInterest>(); // Or do this in the constructor.
    }
}
