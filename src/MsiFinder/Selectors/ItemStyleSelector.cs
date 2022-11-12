// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System.Windows;
using System.Windows.Controls;
using MsiFinder.ViewModel;

namespace MsiFinder.Selectors;

public class ItemStyleSelector : StyleSelector
{
    public Style ProductStyle { get; set; }

    public Style ComponentStyle { get; set; }

    public Style TextItemStyle { get; set; }

    public override Style SelectStyle(object item, DependencyObject container)
    {
        return item switch
        {
            ProductViewModel => ProductStyle,
            ComponentViewModel => ComponentStyle,
            TextItemViewModel => TextItemStyle,
            _ => null,
        };
    }
}
