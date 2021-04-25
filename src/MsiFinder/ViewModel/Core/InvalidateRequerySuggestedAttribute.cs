using System;

namespace MsiFinder.ViewModel.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InvalidateRequerySuggestedAttribute : Attribute
    {
    }
}