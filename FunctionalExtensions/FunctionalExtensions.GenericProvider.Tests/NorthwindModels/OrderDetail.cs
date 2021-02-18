using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FunctionalExtensions.GenericProvider.Tests.NorthwindModels
{
    [Table("OrderDetail")]
    public partial class OrderDetail
    {
        [Key]
        [Column(TypeName = "VARCHAR(8000)")]
        public string Id { get; set; }
        public long OrderId { get; set; }
        public long ProductId { get; set; }
        [Required]
        [Column(TypeName = "DECIMAL")]
        public byte[] UnitPrice { get; set; }
        public long Quantity { get; set; }
        [Column(TypeName = "DOUBLE")]
        public double Discount { get; set; }
    }
}
