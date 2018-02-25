using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }


        // We added this field afterwards, then use Nuget Package Manager Console: Add-Migration <new name> to create a new migration. When run,
        // this added the field to the database. Each migration has an Up() and Down() method to apply and remove each change.
        [MaxLength(200)]
        public string Description { get; set; }

        // Convention based...a navigation property discovered on the type. Considered this if type it points to ISN'T a scalar type. Relationship is created.
        // It will automatically target the primary key of the principle entity
        public City City { get; set; }      
        // The dependent class doesn't need to define this key, but it is recommended.
        public int CityId { get; set; }

        // Annotation method
        //[ForeignKey("CityId")]
        //public City City { get; set; }
        //// The dependent class doesn't need to define this key, but it is recommended.
        //public int CityId { get; set; }


    }
}
