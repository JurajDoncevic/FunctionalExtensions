using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FunctionalExtensions.GenericProvider.Tests.NorthwindModels
{
    [Table("EmployeeTerritory")]
    public partial class EmployeeTerritory
    {
        [Key]
        [Column(TypeName = "VARCHAR(8000)")]
        public string Id { get; set; }
        public long EmployeeId { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string TerritoryId { get; set; }
    }
}
