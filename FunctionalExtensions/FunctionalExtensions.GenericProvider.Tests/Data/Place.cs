using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FunctionalExtensions.GenericProvider;
using Microsoft.EntityFrameworkCore;

namespace FunctionalExtensions.GenericProvider.Tests.Data
{
    public partial class Place : BaseModel<long>
    {
        public Place()
        {
            People = new HashSet<Person>();
        }

        [Required]
        public string Name { get; set; }
        public long CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        [InverseProperty("Places")]
        public virtual Country Country { get; set; }
        [InverseProperty(nameof(Person.Place))]
        public virtual ICollection<Person> People { get; set; }
    }
}
