using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FunctionalExtensions.GenericProvider;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FunctionalExtensions.Tests.PeopleModels
{
    public partial class Job : BaseModel<long>
    {
        public Job()
        {
            People = new HashSet<Person>();
        }

        [Required]
        public string Name { get; set; }

        [InverseProperty(nameof(Person.Job))]
        public virtual ICollection<Person> People { get; set; }
    }
}
