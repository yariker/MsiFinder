// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using MvvmMicro;

namespace MsiFinder.ViewModel.Core
{
    public abstract class ValidatingViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, string> _errors = new();
        private Dictionary<string, PropertyInfo> _properties;

        /// <inheritdoc />
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <inheritdoc />
        public bool HasErrors => _errors.Count > 0;

        private Dictionary<string, PropertyInfo> Properties =>
            _properties ??= GetType().GetProperties(BindingFlags.Instance |
                                                    BindingFlags.FlattenHierarchy |
                                                    BindingFlags.Public)
                .ToDictionary(x => x.Name);

        public bool Validate()
        {
            var valid = true;
            foreach (PropertyInfo property in Properties.Values)
            {
                if (property.GetCustomAttribute<ValidateAttribute>(true) != null)
                {
                    ClearError(property.Name);

                    if (OnValidate(property.Name) is { Status: ValidationStatus.Failed } result)
                    {
                        SetError(property.Name, result.ErrorMessage);
                        valid = false;
                        break;
                    }
                }
            }

            return valid;
        }

        /// <inheritdoc />
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return _errors.Values;
            }

            return _errors.TryGetValue(propertyName, out string error)
                ? EnumerableEx.Return(error)
                : Enumerable.Empty<string>();
        }

        protected virtual ValidationResult OnValidate(string propertyName) => ValidationResult.Valid;

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName != null)
            {
                ClearError(propertyName);

                if (Properties.TryGetValue(propertyName, out PropertyInfo propertyInfo) &&
                    propertyInfo.GetCustomAttribute<InvalidateRequerySuggestedAttribute>() != null)
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        protected void SetError(string propertyName, string error)
        {
            if (_errors.TryGetValue(propertyName, out string oldError))
            {
                if (oldError == error)
                {
                    return;
                }

                _errors[propertyName] = error;
            }
            else
            {
                _errors.Add(propertyName, error);

                if (_errors.Count == 1)
                {
                    OnPropertyChanged(nameof(HasErrors));
                }
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void ClearError(string propertyName)
        {
            if (_errors.Remove(propertyName))
            {
                if (_errors.Count == 0)
                {
                    OnPropertyChanged(nameof(HasErrors));
                }

                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }
    }
}
