// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace MsiFinder.Model;

public class Component : Record
{
    private string _registryKey;

    public Component(Guid code, InstallContext context, string sid)
        : base(code, context, sid)
    {
    }

    public override string RegistryKey => _registryKey ??= GetRegistryKey();

    public static IEnumerable<Component> GetComponents()
    {
        for (int index = 0; ; index++)
        {
            Component component = MsiHelper.MsiEnumComponents(null, InstallContext.Machine, index);
            if (component != null)
            {
                yield return component;
            }
            else
            {
                break;
            }
        }
    }

    public IEnumerable<Product> GetProducts()
    {
        for (int index = 0; ; index++)
        {
            Product product = MsiHelper.MsiEnumClients(Code, Sid, Context, index);
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

    public string GetPath(Product product) => MsiHelper.MsiGetComponentPath(
        product.Code, Code, product.Sid, product.Context);

    private string GetRegistryKey()
    {
        string sid = Sid ?? SystemSid;
        return Registry.LocalMachine.Name +
               $@"\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\{sid}\Components\{PackedCode}";
    }
}
