using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FunctionalExtensions.GenericProvider;
using Microsoft.EntityFrameworkCore;

namespace FunctionalExtensions.GenericProvider.Tests.Data
{
    public partial class Country : BaseModel<long>
    {
        public Country()
        {
            Places = new HashSet<Place>();
        }

        [Required]
        public string Name { get; set; }

        [InverseProperty(nameof(Place.Country))]
        public virtual ICollection<Place> Places { get; set; }
    }
}
