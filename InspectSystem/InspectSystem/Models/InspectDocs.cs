using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectDocs")]
    public class InspectDocs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "表單編號")]
        public int DocID { get; set; }
        [Required]
        [Display(Name = "日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Date { get; set; }
        [Display(Name = "完成時間")]
        [DataType(DataType.Time)]
        public DateTime? EndTime { get; set; }
        [Required]
        [ForeignKey("InspectAreas")]
        [Display(Name = "區域代碼")]
        public int AreaID { get; set; }
        [NotMapped]
        [Display(Name = "區域名稱")]
        public string AreaName { get; set; }
        [Required]
        [Display(Name = "巡檢人員ID")]
        public int UserID { get; set; }
        [Required]
        [Display(Name = "巡檢人員名稱")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "簽核主管ID")]
        public int CheckerID { get; set; }
        [Required]
        [Display(Name = "簽核主管名稱")]
        public string CheckerName { get; set; }

        public virtual InspectAreas InspectAreas { get; set; }
    }
}