// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Windows;
using System.Windows.Input;
using MvvmMicro;

namespace MsiFinder.ViewModel
{
    public class PropertyViewModel : ViewModelBase
    {
        public PropertyViewModel(string property, string value)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Value = value;

            if (!string.IsNullOrEmpty(value))
            {
                CopyCommand = new RelayCommand(Copy);
            }
        }

        public string Property { get; }

        public string Value { get; }

        public ICommand CopyCommand { get; }

        private void Copy() => Clipboard.SetText(Value);
    }
}
