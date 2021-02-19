using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FunctionalExtensions.GenericProvider;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FunctionalExtensions.Tests.NorthwindModels
{
    [Table("Category")]
    public partial class Category : BaseModel<long>
    {
        /* Can be removed because this data exists in the BaseModel.
         * If the model has to extend the description of the Id, then hide/mask it with the Id property and attributes. */ 
        //[Key]
        //public long Id { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string CategoryName { get; set; }
        [Column(TypeName = "VARCHAR(8000)")]
        public string Description { get; set; }
    }
}
