// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace MsiFinder.Model;

public class Product : Record
{
    private readonly Lazy<string> _location;
    private string _name;
    private string _version;
    private string _registryKey;

    public Product(Guid code, InstallContext context, string sid)
        : base(code, context, sid)
    {
        _location = new Lazy<string>(() => GetProperty(InstallProperty.InstallLocation));
    }

    public string Name => _name ??= GetProperty(InstallProperty.ProductName);

    public string Version => _version ??= GetProperty(InstallProperty.VersionString);

    public string Location => _location.Value;

    public override string RegistryKey => _registryKey ??= GetRegistryKey();

    public static IEnumerable<Product> GetProducts(Guid? upgradeCode = null)
    {
        for (int index = 0; ; index++)
        {
            Product product = upgradeCode is Guid code
                ? MsiHelper.MsiEnumRelatedProductsEx(code, index)
                : MsiHelper.MsiEnumProductsEx(index);

            if (product != null)
            {
                yield return product;
            }
            else
            {
                break;
            }
        }
    }

    public string GetProperty(string property) => MsiHelper.MsiGetProductInfoEx(Code, Sid, Context, property);

    private string GetRegistryKey()
    {
        string sid = Sid ?? SystemSid;
        return Registry.LocalMachine.Name +
               $@"\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\{sid}\Products\{PackedCode}";
    }
}
