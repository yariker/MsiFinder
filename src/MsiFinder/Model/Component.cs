using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Microsoft.Win32;
using static MsiFinder.Model.NativeMethods;
using static MsiFinder.Model.NativeMethods.ERROR;

namespace MsiFinder.Model
{
    public record Component : Record
    {
        public Component(Guid code, InstallContext context, string sid)
            : base(code, context, sid)
        {
        }

        public override string RegistryKey
        {
            get
            {
                string sid = Sid ?? SystemSid;
                return Registry.LocalMachine.Name +
                       $@"\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\{sid}\Components\{PackedCode}";
            }
        }

        public static IEnumerable<Component> GetComponents()
        {
            var componentCode = new StringBuilder(CodeLength);
            var userSid = new StringBuilder(MAX_PATH);
            int index = 0;

            while (true)
            {
                int userIdSize = userSid.Capacity;
                ERROR result = MsiEnumComponentsEx(
                    null,
                    InstallContext.Machine,
                    index++,
                    componentCode,
                    out InstallContext context,
                    userSid,
                    ref userIdSize);
                
                if (result == ERROR_SUCCESS)
                {
                    yield return new Component(
                        code: Guid.Parse(componentCode.ToString()),
                        context: context,
                        sid: context != InstallContext.Machine ? userSid.ToString() : null);
                }
                else if (result == ERROR_NO_MORE_ITEMS)
                {
                    break;
                }
                else
                {
                    throw new Win32Exception((int)result);
                }
            }
        }

        public string GetPath(Product product)
        {
            var stringBuilder = new StringBuilder(MAX_PATH);
            int length = stringBuilder.Capacity;

            MsiGetComponentPathEx(
                GuidToCode(product.Code),
                GuidToCode(Code),
                product.Sid,
                product.Context,
                stringBuilder,
                ref length);

            return stringBuilder.Length > 0 ? stringBuilder.ToString() : null;
        }

        public IEnumerable<Product> GetProducts()
        {
            var productCodeBuilder = new StringBuilder(CodeLength);
            var userSidBuilder = new StringBuilder(MAX_PATH);
            int index = 0;

            while (true)
            {
                int userIdSize = userSidBuilder.Capacity;
                ERROR result = MsiEnumClientsEx(
                    GuidToCode(Code),
                    Sid,
                    Context,
                    index++,
                    productCodeBuilder,
                    out InstallContext context,
                    userSidBuilder,
                    ref userIdSize);

                if (result == ERROR_SUCCESS)
                {
                    yield return new Product(
                        code: Guid.Parse(productCodeBuilder.ToString()),
                        context: context,
                        sid: context != InstallContext.Machine ? userSidBuilder.ToString() : null);
                }
                else if (result == ERROR_NO_MORE_ITEMS)
                {
                    break;
                }
                else
                {
                    throw new Win32Exception((int)result);
                }
            }
        }
    }
}