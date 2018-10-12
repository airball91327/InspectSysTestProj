using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectSystem.Models
{
    [Table("InspectMemberAreas")]
    public class InspectMemberAreas
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [ForeignKey("InspectMembers")]
        [Display(Name = "員工代號")]
        public int MemberId { get; set; }
        [Required]
        [ForeignKey("InspectAreas")]
        [Display(Name = "巡檢區域")]
        public int ChargeAreaId { get; set; }

        public virtual InspectAreas InspectAreas { get; set; }
        public virtual InspectMembers InspectMembers { get; set; }
    }
}