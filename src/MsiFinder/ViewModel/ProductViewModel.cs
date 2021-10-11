// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MsiFinder.Model;
using MsiFinder.ViewModel.Messages;
using MvvmMicro;
using Component = MsiFinder.Model.Component;

namespace MsiFinder.ViewModel
{
    public class ProductViewModel : RecordViewModel<Product>
    {
        public ProductViewModel(Product model)
            : base(model)
        {
            ShowDetailsCommand = new RelayCommand(ShowDetails, () => Model.CheckExists());
        }

        public override string Location => Model.Location;

        public ICommand ShowDetailsCommand { get; }

        protected override async Task LoadAsync()
        {
            Component[] components = await Task.Run(() => Component.GetComponents().ToArray());
            foreach (Component component in components)
            {
                Product[] products = await Task.Run(() => component.GetProducts().ToArray());
                if (products.Contains(Model))
                {
                    Items.Insert(Items.Count - 1, new ComponentViewModel(component, component.GetPath(Model), false));
                }
            }
        }

        protected override bool CanRepair() => Model.CheckExists();

        protected override bool CanUninstall() => Model.CheckExists();

        protected override void Repair()
        {
            try
            {
                Process.Start(new ProcessStartInfo("msiexec")
                {
                    Arguments = $"/f {Model.Code:B}",
                    UseShellExecute = true,
                });
            }
            catch (Win32Exception)
            {
                // Don't care.
            }
        }

        protected override void Uninstall()
        {
            try
            {
                Process.Start(new ProcessStartInfo("msiexec")
                {
                    Arguments = $"/x {Model.Code:B}",
                    UseShellExecute = true,
                });
            }
            catch (Win32Exception)
            {
                // Don't care.
            }
        }

        private void ShowDetails()
        {
            var viewModel = new ProductDetailsViewModel(Model);
            Messenger.Publish(new ShowViewMessage<ProductDetailsViewModel>(viewModel));
        }
    }
}
