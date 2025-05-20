
using System.ComponentModel.DataAnnotations;

namespace Entities.EntityType.Concrete
{
    public abstract class EntityBase<T>
    {
        [Key]
        public virtual T Id { get; set; }
    }
}
