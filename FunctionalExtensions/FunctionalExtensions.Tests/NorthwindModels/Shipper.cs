using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FunctionalExtensions.Tests.NorthwindModels
{
    [Table("Shipper")]
    public partial class Shipper
    {
        [Key]
        public long Id { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string CompanyName { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string Phone { get; set; }
    }
}
