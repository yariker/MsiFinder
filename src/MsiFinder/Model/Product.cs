using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Microsoft.Win32;
using static MsiFinder.Model.NativeMethods;
using static MsiFinder.Model.NativeMethods.ERROR;

namespace MsiFinder.Model
{
    public record Product : Record
    {
        public Product(Guid code, InstallContext context, string sid)
            : base(code, context, sid)
        {
        }

        public string Name => GetProperty(INSTALLPROPERTY_PRODUCTNAME);

        public string Version => GetProperty(INSTALLPROPERTY_VERSIONSTRING);

        public string Location => GetProperty(INSTALLPROPERTY_INSTALLLOCATION);

        public override string RegistryKey
        {
            get
            {
                string sid = Sid ?? SystemSid;
                return Registry.LocalMachine.Name +
                       $@"\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\{sid}\Products\{PackedCode}";
            }
        }

        private string GetProperty(string property)
        {
            var stringBuilder = new StringBuilder(MAX_PATH);
            int length = stringBuilder.Capacity;

            ERROR result = MsiGetProductInfoEx(
                GuidToCode(Code),
                Sid,
                Context,
                property,
                stringBuilder,
                ref length);

            if (length == 0 || result == ERROR_UNKNOWN_PROPERTY)
            {
                return null;
            }

            return result is ERROR_SUCCESS
                ? stringBuilder.ToString()
                : throw new Win32Exception((int)result);
        }

        public static IEnumerable<Product> GetProducts()
        {
            var productCodeBuilder = new StringBuilder(CodeLength);
            var userSidBuilder = new StringBuilder(MAX_PATH);
            int index = 0;

            while (true)
            {
                int userIdSize = userSidBuilder.Capacity;
                ERROR result = MsiEnumProductsEx(
                    null,
                    null,
                    InstallContext.All,
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