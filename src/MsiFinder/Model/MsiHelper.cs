// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.ComponentModel;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.ApplicationInstallationAndServicing;

namespace MsiFinder.Model
{
    internal static class MsiHelper
    {
        private const int CodeSize = 38;

        public static unsafe string MsiGetComponentPath(
            Guid productCode,
            Guid componentCode,
            string userId,
            InstallContext context)
        {
            uint pathSize = 0;

            INSTALLSTATE state = PInvoke.MsiGetComponentPathEx(
                szProductCode: GuidToCode(productCode),
                szComponentCode: GuidToCode(componentCode),
                szUserSid: userId,
                dwContext: (MSIINSTALLCONTEXT)context,
                lpOutPathBuffer: null,
                pcchOutPathBuffer: &pathSize);

            if (state != INSTALLSTATE.INSTALLSTATE_MOREDATA)
            {
                return null;
            }

            if (pathSize == 0)
            {
                return string.Empty;
            }

            // Increase buffer for terminating NULL character.
            pathSize++;

            var path = stackalloc char[(int)pathSize];

            state = PInvoke.MsiGetComponentPathEx(
                szProductCode: GuidToCode(productCode),
                szComponentCode: GuidToCode(componentCode),
                szUserSid: userId,
                dwContext: (MSIINSTALLCONTEXT)context,
                lpOutPathBuffer: path,
                pcchOutPathBuffer: &pathSize);

            return new string(path, 0, (int)pathSize);
        }

        public static unsafe Component MsiEnumComponents(string userId, InstallContext context, int index)
        {
            var ownerIdSize = PInvoke.MAX_PATH;
            var ownerId = stackalloc char[(int)ownerIdSize];
            var componentCode = stackalloc char[CodeSize + 1];
            var installContext = MSIINSTALLCONTEXT.MSIINSTALLCONTEXT_NONE;

            var result = (WIN32_ERROR)PInvoke.MsiEnumComponentsEx(
                szUserSid: userId,
                dwContext: (uint)context,
                dwIndex: (uint)index,
                szInstalledComponentCode: componentCode,
                pdwInstalledContext: &installContext,
                szSid: ownerId,
                pcchSid: &ownerIdSize);

            switch (result)
            {
                case WIN32_ERROR.NO_ERROR:
                    return new Component(
                        code: Guid.Parse(new string(componentCode, 0, CodeSize)),
                        context: (InstallContext)installContext,
                        sid: GetSid(installContext, ownerId, ownerIdSize));

                case WIN32_ERROR.ERROR_NO_MORE_ITEMS:
                    return null;

                default:
                    throw new Win32Exception((int)result);
            }
        }

        public static unsafe Product MsiEnumProductsEx(int index)
        {
            var ownerIdSize = PInvoke.MAX_PATH;
            var ownerId = stackalloc char[(int)ownerIdSize];
            var productCode = stackalloc char[CodeSize + 1];
            var installContext = MSIINSTALLCONTEXT.MSIINSTALLCONTEXT_NONE;

            var result = (WIN32_ERROR)PInvoke.MsiEnumProductsEx(
                null,
                (string)null,
                (uint)MSIINSTALLCONTEXT.MSIINSTALLCONTEXT_ALL,
                (uint)index,
                productCode,
                &installContext,
                ownerId,
                &ownerIdSize);

            switch (result)
            {
                case WIN32_ERROR.NO_ERROR:
                    return new Product(
                        code: Guid.Parse(new string(productCode, 0, CodeSize)),
                        context: (InstallContext)installContext,
                        sid: GetSid(installContext, ownerId, ownerIdSize));

                case WIN32_ERROR.ERROR_NO_MORE_ITEMS:
                    return null;

                default:
                    throw new Win32Exception((int)result);
            }
        }

        public static unsafe Product MsiEnumClients(Guid componentCode, string userId, InstallContext context, int index)
        {
            var ownerIdSize = PInvoke.MAX_PATH;
            var ownerId = stackalloc char[(int)ownerIdSize - 1];
            var productCode = stackalloc char[CodeSize + 1];
            var installContext = MSIINSTALLCONTEXT.MSIINSTALLCONTEXT_NONE;

            var result = (WIN32_ERROR)PInvoke.MsiEnumClientsEx(
                szComponent: GuidToCode(componentCode),
                szUserSid: userId,
                dwContext: (MSIINSTALLCONTEXT)context,
                dwProductIndex: (uint)index,
                szProductBuf: productCode,
                pdwInstalledContext: &installContext,
                szSid: ownerId,
                pcchSid: &ownerIdSize);

            switch (result)
            {
                case WIN32_ERROR.NO_ERROR:
                    return new Product(
                        code: Guid.Parse(new string(productCode, 0, CodeSize)),
                        context: (InstallContext)installContext,
                        sid: GetSid(installContext, ownerId, ownerIdSize));

                case WIN32_ERROR.ERROR_NO_MORE_ITEMS:
                    return null;

                default:
                    throw new Win32Exception((int)result);
            }
        }

        public static unsafe string MsiGetProductInfoEx(
            Guid productCode,
            string userId,
            InstallContext context,
            string property)
        {
            uint valueSize = 0;

            var result = (WIN32_ERROR)PInvoke.MsiGetProductInfoEx(
                szProductCode: GuidToCode(productCode),
                szUserSid: userId,
                dwContext: (MSIINSTALLCONTEXT)context,
                szProperty: property,
                szValue: null,
                pcchValue: &valueSize);

            if (result == WIN32_ERROR.ERROR_UNKNOWN_PROPERTY)
            {
                return null;
            }

            if (result is not WIN32_ERROR.ERROR_SUCCESS and not WIN32_ERROR.ERROR_MORE_DATA)
            {
                throw new Win32Exception((int)result);
            }

            if (valueSize == 0)
            {
                return string.Empty;
            }

            // Increase buffer for terminating NULL character.
            valueSize++;

            char* value = stackalloc char[(int)valueSize];

            result = (WIN32_ERROR)PInvoke.MsiGetProductInfoEx(
                szProductCode: GuidToCode(productCode),
                szUserSid: userId,
                dwContext: (MSIINSTALLCONTEXT)context,
                szProperty: property,
                szValue: value,
                pcchValue: &valueSize);

            return result == WIN32_ERROR.ERROR_SUCCESS
                ? new string(value, 0, (int)valueSize)
                : throw new Win32Exception((int)result);
        }

        private static unsafe string GetSid(MSIINSTALLCONTEXT installContext, char* sid, uint sidSize) =>
            installContext != MSIINSTALLCONTEXT.MSIINSTALLCONTEXT_MACHINE ? new string(sid, 0, (int)sidSize) : null;

        private static string GuidToCode(Guid guid) => guid.ToString("B").ToUpper();
    }
}
