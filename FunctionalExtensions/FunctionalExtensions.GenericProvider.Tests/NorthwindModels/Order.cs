using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FunctionalExtensions.GenericProvider.Tests.NorthwindModels
{
    [Table("Order")]
    public partial class Order
    {
        [Key]
        public long Id { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string CustomerId { get; set; }
        public long EmployeeId { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string OrderDate { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string RequiredDate { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string ShippedDate { get; set; }
        public long? ShipVia { get; set; }
        [Required]
        [Column(TypeName = "DECIMAL")]
        public byte[] Freight { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string ShipName { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string ShipAddress { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string ShipCity { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string ShipRegion { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string ShipPostalCode { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string ShipCountry { get; set; }
    }
}
