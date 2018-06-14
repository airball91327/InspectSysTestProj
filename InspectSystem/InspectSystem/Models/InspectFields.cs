using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectFields")]
    public class InspectFields
    {
        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "ID")]
        public int ID { get; set; }         //對應ClassesOfAreas的ID、程式產生
        [Key, Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "項目代碼")]
        public int ItemID { get; set; }
        [Key, Column(Order = 3)]
        [Required]
        [Display(Name = "欄位代碼")]
        public int FieldID { get; set; }
        [Required]
        [Display(Name = "欄位名稱")]
        public string FieldName { get; set; }
        [Required]
        [Display(Name = "資料型態")]
        public string DataType { get; set; }
        [Display(Name = "單位")]
        public string UnitOfData { get; set; }
        [Display(Name = "最小值")]
        public double MinValue { get; set; }
        [Display(Name = "最大值")]
        public double MaxValue { get; set; }

    }
}