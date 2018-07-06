using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectDocDetails")]
    public class InspectDocDetails
    {
        [Key, Column(Order = 1)]
        [Required]
        [Display(Name = "表單編號")]
        public int DocID { get; set; }
        [Required]
        [Display(Name = "區域代碼")]
        public int AreaID { get; set; }
        [Required]
        [Display(Name = "區域名稱")]
        public string AreaName { get; set; }
        [Key, Column(Order = 2)]
        [Required]
        [Display(Name = "類別代碼")]
        public int ClassID { get; set; }
        [Required]
        [Display(Name = "區域名稱")]
        public string ClassName { get; set; }
        [Key, Column(Order = 3)]
        [Required]
        [Display(Name = "項目代碼")]
        public int ItemID { get; set; }
        [Required]
        [Display(Name = "項目名稱")]
        public string ItemName { get; set; }
        [Key, Column(Order = 4)]
        [Required]
        [Display(Name = "欄位代碼")]
        public int FieldID { get; set; }
        [Display(Name = "欄位名稱")]
        public string FieldName { get; set; }
        [Display(Name = "單位")]
        public string UnitOfData { get; set; }
        [Display(Name = "數值")]
        public string Value { get; set; }
        [Display(Name = "是否正常")]
        public Boolean IsFunctional { get; set; }
        [Display(Name = "錯誤描述")]
        public string ErrorDescription { get; set; }
        [Display(Name = "維修單號")]
        public string RepairDocID { get; set; }

    }
}