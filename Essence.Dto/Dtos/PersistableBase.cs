using System;

namespace Essence.Dto
{
    public abstract class PersistableBase
    {
        public virtual Guid Id { get; set; }
        public virtual byte[] Version { get; set; }
        public virtual DateTime? Ts { get; set; }
        public virtual DateTime? CreationDate { get; set; }
    }
}
