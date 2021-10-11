// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System.Collections.ObjectModel;
using MvvmMicro;

namespace MsiFinder.ViewModel
{
    public abstract class ItemViewModel : ViewModelBase
    {
        public ObservableCollection<ItemViewModel> Items { get; } = new();
    }
}
