using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace M3alam.Common.Entities
{
    public abstract class BaseEntity<TId> : IdBasedEntity<TId>, IDeletableEntity, IAuditableEntity<string>
    {
        public virtual DateTime Created { get; set; }
        public virtual DateTime? LastModified { get; set; }
        public virtual string? CreatedBy { get; set; }
        public virtual string? LastModifiedBy { get; set; }
        public virtual bool? IsDeleted { get; set; }

        [JsonIgnore]
        public byte[]? RowVersion { get; set; }
    }
}
