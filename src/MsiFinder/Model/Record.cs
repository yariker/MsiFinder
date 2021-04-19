using System;
using System.Linq;

namespace MsiFinder.Model
{
    public abstract record Record
    {
        public const int CodeLength = 39;
        public const string SystemSid = "S-1-5-18";

        protected Record(Guid code, InstallContext context, string sid)
        {
            Code = code;
            Context = context;
            Sid = sid;
        }

        public Guid Code { get; }

        public InstallContext Context { get;}

        public string Sid { get; }

        public string PackedCode => string.Concat(Code.ToByteArray().SelectMany(x => x.ToString("X2").Reverse()));

        public abstract string RegistryKey { get; }

        public virtual bool Equals(Record other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Code.Equals(other.Code) &&
                   Context == other.Context &&
                   string.Equals(Sid, other.Sid, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Code.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Context;
                hashCode = (hashCode * 397) ^ (Sid != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Sid) : 0);
                return hashCode;
            }
        }

        protected static string GuidToCode(Guid guid) => guid.ToString("B").ToUpper();
    }
}