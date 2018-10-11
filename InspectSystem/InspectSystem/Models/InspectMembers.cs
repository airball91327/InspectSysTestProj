using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectMembers")]
    public class InspectMembers
    {
        [Key]
        [Required]
        [Display(Name = "員工代號")]
        public int MemberId { get; set; }
        [Display(Name = "員工姓名")]
        public string MemberName { get; set; }
        [Display(Name = "所屬部門")]
        public string Department { get; set; }
        [ForeignKey("InspectAreas")]
        [Display(Name = "巡檢區域1代碼")]
        public int InspectArea1_ID { get; set; }
        [Display(Name = "巡檢區域1")]
        public string InspectArea1_Name { get; set; }
        [ForeignKey("InspectAreas")]
        [Display(Name = "巡檢區域2代碼")]
        public int InspectArea2_ID { get; set; }
        [Display(Name = "巡檢區域2")]
        public string InspectArea2_Name { get; set; }
        [ForeignKey("InspectAreas")]
        [Display(Name = "巡檢區域3代碼")]
        public int InspectArea3_ID { get; set; }
        [Display(Name = "巡檢區域3")]
        public string InspectArea3_Name { get; set; }

        public virtual InspectAreas InspectAreas { get; set; }
    }
}