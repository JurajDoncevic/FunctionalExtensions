using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FunctionalExtensions.GenericProvider
{
    /// <summary>
    /// Base abstract model all Provider models must inherit
    /// </summary>
    /// <typeparam name="TKey">Type of model PK Id</typeparam>
    public abstract class BaseModel<TKey>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; set; }
    }
}
