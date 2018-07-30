﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectDocFlow")]
    public class InspectDocFlow
    {
        [Key, Column(Order = 1)]
        [Display(Name = "表單編號")]
        public int DocID { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "關卡號")]
        public int StepID { get; set; }
        [Required]
        [Display(Name = "巡檢人員ID")]
        public int UserID { get; set; }
        [NotMapped]
        [Display(Name = "巡檢人員")]
        public string UserName { get; set; }
        [Display(Name = "意見描述")]
        public string Opinions { get; set; }
        [Required]
        [ForeignKey("InspectFlowStatusTable")]
        [Display(Name = "流程狀態編號")]
        public int FlowStatusID { get; set; }
        [Required]
        [Display(Name ="異動人員ID")]
        public int EditorID { get; set; }
        [Display(Name ="異動人員")]
        public string EditorName { get; set; }
        [Display(Name ="異動時間")]
        public DateTime? EditTime { get; set; }

        public virtual InspectFlowStatusTable InspectFlowStatusTable { get; set; }
    }
}