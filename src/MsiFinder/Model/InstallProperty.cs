// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MsiFinder.Model;

public static class InstallProperty
{
    public const string AssignmentType = "AssignmentType";
    public const string AuthorizedLuaApp = "AuthorizedLUAApp";
    public const string DiskPrompt = "DiskPrompt";
    public const string DisplayName = "DisplayName";
    public const string HelpLink = "HelpLink";
    public const string HelpTelephone = "HelpTelephone";
    public const string InstallDate = "InstallDate";
    public const string InstalledLanguage = "InstalledLanguage";
    public const string InstalledProductName = "InstalledProductName";
    public const string InstallLocation = "InstallLocation";
    public const string InstallSource = "InstallSource";
    public const string InstanceType = "InstanceType";
    public const string Language = "Language";
    public const string LastUsedSource = "LastUsedSource";
    public const string LastUsedType = "LastUsedType";
    public const string LocalPackage = "LocalPackage";
    public const string LuaEnabled = "LUAEnabled";
    public const string MediaPackagePath = "MediaPackagePath";
    public const string MoreInfoUrl = "MoreInfoURL";
    public const string PackageCode = "PackageCode";
    public const string PackageName = "PackageName";
    public const string PatchState = "State";
    public const string PatchType = "PatchType";
    public const string ProductIcon = "ProductIcon";
    public const string ProductId = "ProductID";
    public const string ProductName = "ProductName";
    public const string ProductState = "State";
    public const string Publisher = "Publisher";
    public const string RegCompany = "RegCompany";
    public const string RegOwner = "RegOwner";
    public const string Transforms = "Transforms";
    public const string Uninstallable = "Uninstallable";
    public const string UrlInfoAbout = "URLInfoAbout";
    public const string UrlUpdateInfo = "URLUpdateInfo";
    public const string Version = "Version";
    public const string VersionMajor = "VersionMajor";
    public const string VersionMinor = "VersionMinor";
    public const string VersionString = "VersionString";

    public static IEnumerable<string> GetProperties()
    {
        return typeof(InstallProperty).GetFields(BindingFlags.Static | BindingFlags.Public).Select(
            field => field.GetRawConstantValue()).Cast<string>();
    }
}
