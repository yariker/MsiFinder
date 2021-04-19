using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace MsiFinder.Model
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    internal static class NativeMethods
    {
        internal const int MAX_PATH = 260;
        internal const int S_OK = 0;

        internal const string INSTALLPROPERTY_PACKAGENAME = "PackageName";
        internal const string INSTALLPROPERTY_TRANSFORMS = "Transforms";
        internal const string INSTALLPROPERTY_LANGUAGE = "Language";
        internal const string INSTALLPROPERTY_PRODUCTNAME = "ProductName";
        internal const string INSTALLPROPERTY_ASSIGNMENTTYPE = "AssignmentType";
        internal const string INSTALLPROPERTY_PACKAGECODE = "PackageCode";
        internal const string INSTALLPROPERTY_VERSION = "Version";
        internal const string INSTALLPROPERTY_PRODUCTICON = "ProductIcon";
        internal const string INSTALLPROPERTY_INSTALLEDPRODUCTNAME = "InstalledProductName";
        internal const string INSTALLPROPERTY_VERSIONSTRING = "VersionString";
        internal const string INSTALLPROPERTY_HELPLINK = "HelpLink";
        internal const string INSTALLPROPERTY_HELPTELEPHONE = "HelpTelephone";
        internal const string INSTALLPROPERTY_INSTALLLOCATION = "InstallLocation";
        internal const string INSTALLPROPERTY_INSTALLSOURCE = "InstallSource";
        internal const string INSTALLPROPERTY_INSTALLDATE = "InstallDate";
        internal const string INSTALLPROPERTY_PUBLISHER = "Publisher";
        internal const string INSTALLPROPERTY_LOCALPACKAGE = "LocalPackage";
        internal const string INSTALLPROPERTY_URLINFOABOUT = "URLInfoAbout";
        internal const string INSTALLPROPERTY_URLUPDATEINFO = "URLUpdateInfo";
        internal const string INSTALLPROPERTY_VERSIONMINOR = "VersionMinor";
        internal const string INSTALLPROPERTY_VERSIONMAJOR = "VersionMajor";

        [DllImport("Msi.dll", CharSet = CharSet.Unicode)]
        internal static extern ERROR MsiEnumComponentsEx(
            string szUserSid,
            InstallContext dwContext,
            int dwIndex,
            StringBuilder szInstalledComponentCode,
            out InstallContext pdwInstalledContext,
            StringBuilder szSid,
            ref int pcchSid);

        [DllImport("Msi.dll", CharSet = CharSet.Unicode)]
        internal static extern ERROR MsiEnumProductsEx(
            string szProductCode,
            string szUserSid,
            InstallContext dwContext,
            int dwIndex,
            StringBuilder szInstalledProductCode,
            out InstallContext pdwInstalledContext,
            StringBuilder szSid,
            ref int pcchSid);

        [DllImport("Msi.dll", CharSet = CharSet.Unicode)]
        internal static extern ERROR MsiEnumClientsEx(
            string szComponent,
            string szUserSid,
            InstallContext dwContext,
            int dwProductIndex,
            StringBuilder szProductBuf,
            out InstallContext pdwInstalledContext,
            StringBuilder szSid,
            ref int pcchSid);

        [DllImport("Msi.dll", CharSet = CharSet.Unicode)]
        internal static extern InstallState MsiGetComponentPathEx(
            string szProductCode,
            string szComponentCode,
            string szUserSid,
            InstallContext dwContext,
            StringBuilder lpOutPathBuffer,
            ref int pcchOutPathBuffer);

        [DllImport("Msi.dll", CharSet = CharSet.Unicode)]
        internal static extern ERROR MsiGetProductInfoEx(
            string szProductCode,
            string szUserSid,
            InstallContext dwContext,
            string szProperty,
            StringBuilder szValue,
            ref int pcchValue);

        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        internal static extern int PathMatchSpecEx(
            string file,
            string spec,
            MatchPatternFlags flags);

        internal enum ERROR
        {
            ERROR_SUCCESS = 0,
            ERROR_NO_MORE_ITEMS = 259,
            ERROR_UNKNOWN_PROPERTY = 1608,
        }

        [Flags]
        internal enum MatchPatternFlags
        {
            Normal = 0,
            Multiple = 1,
            DontStripSpaces = 65536,
        }
    }
}