using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BizBook.Common.Library.Models.Entities
{
    public abstract class Entity : IEntity
    {
        [Key]
        [Column(TypeName = "varchar(128)")]
        public string Id { get; set; }

        [IsIndex]
        [Required]
        public DateTime Created { get; set; }

        [IsIndex]
        [Required]
        [Column(TypeName = "varchar(128)")]
        public string CreatedBy { get; set; }

        [IsIndex]
        [Required]
        [Column(TypeName = "varchar(32)")]
        public string CreatedFrom { get; set; }

        [IsIndex]
        [Required]
        public DateTime Modified { get; set; }

        [Required]
        [Column(TypeName = "varchar(128)")]
        public string ModifiedBy { get; set; }

        [IsIndex]
        [Required]
        public bool IsActive { get; set; }

        [IsIndex]
        [Required]
        [Column(TypeName = "varchar(128)")]
        public string ShopId { get; set; }

        [IsIndex]
        public bool IsDeleted { get; set; }
    }
}