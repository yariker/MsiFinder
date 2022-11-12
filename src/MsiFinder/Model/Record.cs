// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Linq;
using Microsoft.Win32;

namespace MsiFinder.Model;

public abstract class Record
{
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

    public bool CheckExists() => Registry.GetValue(RegistryKey, string.Empty, string.Empty) != null;

    public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is Record other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Code.GetHashCode();
            hashCode = (hashCode * 397) ^ (int)Context;
            hashCode = (hashCode * 397) ^ (Sid != null ? Sid.GetHashCode() : 0);
            return hashCode;
        }
    }

    protected bool Equals(Record other) => Code.Equals(other.Code) && Context == other.Context && Sid == other.Sid;
}
