using System;
using System.Collections.Generic;
using System.Linq;

namespace StocksApi.Model
{
    public abstract class Entity : IEquatable<Entity>
    {
        public Guid Id { get; set; }

        public bool Equals(Entity other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            return Equals((Entity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}