using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.EntityType.Concrete
{
    public abstract class GeographicEntities : EntityBase<int>
    {
        public virtual decimal? Latitude { get; set; }
        public virtual decimal? Longitude { get; set; }
    }
}
