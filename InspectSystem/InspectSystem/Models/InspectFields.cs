using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectFields")]
    public class InspectFields
    {
        [Key]
        [Required]
        [Display(Name = "欄位代碼")]
        public int FieldID { get; set; }
        [Required]
        [Display(Name = "欄位名稱")]
        public string FieldName { get; set; }
        [Required]
        [Display(Name = "項目代碼")]
        public int ItemID { get; set; }
        [Required]
        [Display(Name = "資料型態")]
        public string DataType { get; set; }
        [Display(Name = "單位")]
        public string UnitOfData { get; set; }
        [Display(Name = "最小值")]
        public float MinValue { get; set; }
        [Display(Name = "最大值")]
        public float MaxValue { get; set; }

    }
}