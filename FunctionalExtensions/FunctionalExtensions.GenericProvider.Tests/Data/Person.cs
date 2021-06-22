using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FunctionalExtensions.GenericProvider;
using Microsoft.EntityFrameworkCore;

namespace FunctionalExtensions.GenericProvider.Tests.Data
{
    public partial class Person : BaseModel<long>
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public long? PlaceId { get; set; }
        public long? Age { get; set; }
        public long JobId { get; set; }

        [ForeignKey(nameof(JobId))]
        [InverseProperty("People")]
        public virtual Job Job { get; set; }
        [ForeignKey(nameof(PlaceId))]
        [InverseProperty("People")]
        public virtual Place Place { get; set; }
    }
}
