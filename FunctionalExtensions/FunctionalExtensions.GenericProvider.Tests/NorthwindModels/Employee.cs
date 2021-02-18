using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FunctionalExtensions.GenericProvider.Tests.NorthwindModels
{
    [Table("Employee")]
    public partial class Employee
    {
        [Key]
        public long Id { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string LastName { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string FirstName { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string Title { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string TitleOfCourtesy { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string BirthDate { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string HireDate { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string Address { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string City { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string Region { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string PostalCode { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string Country { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string HomePhone { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string Extension { get; set; }
        public byte[] Photo { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string Notes { get; set; }
        public long? ReportsTo { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string PhotoPath { get; set; }
    }
}
