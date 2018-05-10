using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("Precautions")]
    public class Precautions
    {
        [Key]
        [Required]
        public int PrecautionID { get; set; }
        [Required]
        [Display(Name = "區域代碼")]
        public int AreaID { get; set; }
        [Display(Name = "注意事項")]
        public string Content { get; set; }
    }
}