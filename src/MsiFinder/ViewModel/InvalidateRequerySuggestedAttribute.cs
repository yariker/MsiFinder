using System;

namespace MsiFinder.ViewModel
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InvalidateRequerySuggestedAttribute : Attribute
    {
    }
}