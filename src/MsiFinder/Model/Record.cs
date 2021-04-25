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

        public InstallContext Context { get; }

        public string Sid { get; }

        public string PackedCode => string.Concat(Code.ToByteArray().SelectMany(x => x.ToString("X2").Reverse()));

        public abstract string RegistryKey { get; }

        protected static string GuidToCode(Guid guid) => guid.ToString("B").ToUpper();
    }
}