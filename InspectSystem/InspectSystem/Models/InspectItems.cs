using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectItems")]
    public class InspectItems
    {
        [Key]
        [Required]
        [Display(Name = "ID")]
        public int ID { get; set; }         //對應ClassesOfAreas的ID
        [Required]
        [Display(Name = "區域代碼")]
        public int AreaID { get; set; }
        [Required]
        [Display(Name = "類別代碼")]
        public int ClassID { get; set; }
        [Key]
        [Required]
        [Display(Name = "項目代碼")]
        public int ItemID { get; set; }
        [Required]
        [Display(Name = "項目名稱")]
        public string ItemName { get; set; }
        [Required]
        [Display(Name = "項目狀態")]
        public Boolean ItemStatus { get; set; }

    }
}