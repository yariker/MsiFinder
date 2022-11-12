// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MsiFinder.Model;
using MvvmMicro;

namespace MsiFinder.ViewModel;

public class ProductDetailsViewModel : ViewModelBase
{
    private readonly Product _product;
    private IReadOnlyList<PropertyViewModel> _items;

    public ProductDetailsViewModel(Product product)
    {
        _product = product ?? throw new ArgumentNullException(nameof(product));
        LoadCommand = new RelayCommand(Load);
    }

    public IReadOnlyList<PropertyViewModel> Items
    {
        get => _items;
        private set => Set(ref _items, value);
    }

    public ICommand LoadCommand { get; }

    private void Load()
    {
        Items = InstallProperty.GetProperties()
                               .Select(property => new PropertyViewModel(property, _product.GetProperty(property)))
                               .ToArray();
    }
}
