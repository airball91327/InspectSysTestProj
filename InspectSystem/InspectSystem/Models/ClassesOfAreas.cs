using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("ClassesOfAreas")]
    public class ClassesOfAreas
    {
        [Key]
        [Required]
        [Display(Name = "ACID")]
        public int ACID { get; set; }
        [Required]
        [ForeignKey("InspectAreas")]
        [Display(Name = "區域代碼")]
        public int AreaID { get; set; }
        [Required]
        [ForeignKey("InspectClasses")]
        [Display(Name = "類別代碼")]
        public int ClassID { get; set; }

        public virtual InspectAreas InspectAreas { get; set; }
        public virtual InspectClasses InspectClasses { get; set; }
    }
}