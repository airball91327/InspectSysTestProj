using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectAreaChecker")]
    public class InspectAreaChecker
    {
        [Key]
        [Required]
        [ForeignKey("InspectAreas")]
        [Display(Name = "區域代碼")]
        public int AreaID { get; set; }
        [Display(Name = "簽核主管ID")]
        public int CheckerID { get; set; }
        [Display(Name = "簽核主管")]
        public string CheckerName { get; set; }

        public virtual InspectAreas InspectAreas { get; set; }
    }
}