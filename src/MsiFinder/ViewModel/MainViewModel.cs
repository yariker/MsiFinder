// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MsiFinder.Model;
using MsiFinder.ViewModel.Core;
using MvvmMicro;
using Windows.Win32;

namespace MsiFinder.ViewModel
{
    public class MainViewModel : ValidatingViewModel
    {
        private const int ItemRate = 5;
        private const int ItemDelay = 100;

        private static readonly char[] InvalidLocationChars = Path.GetInvalidPathChars()
            .Except(new[] { '*', '?' })
            .ToArray();

        private CancellationTokenSource _cancellationTokenSource;
        private string _searchQuery;
        private SearchFor _searchFor;
        private SearchBy _searchBy;
        private bool _isBusy;

        public MainViewModel()
        {
            SearchCommand = new RelayCommand(Search);
            StopCommand = new RelayCommand(Stop);
        }

        [Validate]
        public string SearchQuery
        {
            get => _searchQuery;
            set => Set(ref _searchQuery, value);
        }

        [InvalidateRequerySuggested]
        public bool IsBusy
        {
            get => _isBusy;
            private set => Set(ref _isBusy, value);
        }

        public SearchFor SearchFor
        {
            get => _searchFor;
            set
            {
                if (Set(ref _searchFor, value))
                {
                    CoerceFilters();
                    ClearError(nameof(SearchQuery));
                }
            }
        }

        public SearchBy SearchBy
        {
            get => _searchBy;
            set
            {
                if (Set(ref _searchBy, value))
                {
                    CoerceFilters();
                    ClearError(nameof(SearchQuery));

                    if (value == SearchBy.None)
                    {
                        SearchQuery = string.Empty;
                    }
                }
            }
        }

        public ICommand SearchCommand { get; }

        public ICommand StopCommand { get; }

        public ObservableCollection<ItemViewModel> Results { get; } = new();

        protected override ValidationResult OnValidate(string propertyName)
        {
            if (propertyName == nameof(SearchQuery))
            {
                switch (SearchBy)
                {
                    case SearchBy.Code:
                        if (!Guid.TryParse(SearchQuery, out _))
                        {
                            return ValidationResult.Invalid("Invalid product/component code");
                        }

                        break;
                    case SearchBy.Name:
                        if (string.IsNullOrWhiteSpace(SearchQuery))
                        {
                            return ValidationResult.Invalid("Invalid product name");
                        }

                        break;
                    case SearchBy.Location:
                        if (string.IsNullOrWhiteSpace(SearchQuery) || SearchQuery.IndexOfAny(InvalidLocationChars) >= 0)
                        {
                            return ValidationResult.Invalid("Invalid product location");
                        }

                        break;
                }
            }

            return base.OnValidate(propertyName);
        }

        private async void Search()
        {
            if (IsBusy || !Validate())
            {
                return;
            }

            using (_cancellationTokenSource = new CancellationTokenSource())
            {
                IsBusy = true;
                Results.Clear();

                try
                {
                    switch (SearchFor)
                    {
                        case SearchFor.Product:
                        {
                            Func<Product, bool> filter;
                            switch (SearchBy)
                            {
                                case SearchBy.Code:
                                    Guid code = Guid.Parse(SearchQuery);
                                    filter = x => x.Code == code;
                                    break;
                                case SearchBy.Name:
                                    filter = x => x.Name?.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0;
                                    break;
                                case SearchBy.Location:
                                    filter = x => PInvoke.PathMatchSpecEx(x.Location, SearchQuery, Constants.PMSF_NORMAL) == Constants.S_OK;
                                    break;
                                default:
                                    filter = _ => true;
                                    break;
                            }

                            await SearchProductAsync(filter);
                            break;
                        }

                        case SearchFor.Component:
                        {
                            Func<Component, bool> filter;
                            switch (SearchBy)
                            {
                                case SearchBy.Code:
                                    Guid code = Guid.Parse(SearchQuery);
                                    filter = x => x.Code == code;
                                    break;
                                default:
                                    filter = _ => true;
                                    break;
                            }

                            await SearchComponentAsync(filter);
                            break;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Canceled.
                }
                catch (Exception ex)
                {
                    Results.Add(new TextItemViewModel { Text = ex.Message, Type = TextItemType.Error });
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task SearchProductAsync(Func<Product, bool> filter)
        {
            await foreach (IList<Product> chunk in Product.GetProducts()
                .ToObservable(Scheduler.Default)
                .Where(filter)
                .Buffer(ItemRate)
                .ToAsyncEnumerable()
                .WithCancellation(_cancellationTokenSource.Token))
            {
                chunk.ForEach(x => Results.Add(new ProductViewModel(x)));
                await Task.Delay(ItemDelay);
            }
        }

        private async Task SearchComponentAsync(Func<Component, bool> filter)
        {
            await foreach (IList<Component> chunk in Component.GetComponents()
                .ToObservable(Scheduler.Default)
                .Where(filter)
                .Buffer(ItemRate)
                .ToAsyncEnumerable()
                .WithCancellation(_cancellationTokenSource.Token))
            {
                chunk.ForEach(x => Results.Add(new ComponentViewModel(x)));
                await Task.Delay(ItemDelay);
            }
        }

        private void Stop()
        {
            if (_cancellationTokenSource is { IsCancellationRequested: false })
            {
                _cancellationTokenSource.Cancel();
            }
        }

        private void CoerceFilters()
        {
            if (SearchFor == SearchFor.Component && SearchBy is SearchBy.Name or SearchBy.Location)
            {
                SearchBy = SearchBy.None;
            }
        }
    }
}
