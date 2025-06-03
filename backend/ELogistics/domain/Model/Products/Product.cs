
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using domain.Model.Users;

namespace domain.Model.Products
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string ProductImageUrl { get; set; }

        [Required, MaxLength(200)]
        public string ProductName { get; set; }

        [Required, MaxLength(50)]
        public string ProductCode { get; set; }

        [MaxLength(100)]
        public string? ProductCategory { get; set; }

        [Column(TypeName = "decimal(11,2)")]
        public decimal ProductMRP { get; set; }

        [Column(TypeName = "decimal(11,2)")]
        public decimal ProductRate { get; set; }

        public int AvailableStocks { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
