using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FunctionalExtensions.GenericProvider.Tests.Models
{
    [Table("Category")]
    public partial class Category
    {
        [Key]
        public long Id { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string CategoryName { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string Description { get; set; }
    }
}
