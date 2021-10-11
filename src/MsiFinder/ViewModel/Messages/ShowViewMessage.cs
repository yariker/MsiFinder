// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using MvvmMicro;

namespace MsiFinder.ViewModel.Messages
{
    public class ShowViewMessage<TViewModel>
        where TViewModel : ViewModelBase
    {
        public ShowViewMessage(TViewModel viewModel)
        {
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        public TViewModel ViewModel { get; }
    }
}
