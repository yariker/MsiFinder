// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System.Linq;
using System.Threading.Tasks;
using MsiFinder.Model;

namespace MsiFinder.ViewModel
{
    public class ComponentViewModel : RecordViewModel<Component>
    {
        public ComponentViewModel(Component model, string location = null, bool expandable = true)
            : base(model, expandable)
        {
            Location = location;
        }

        public override string Location { get; }

        protected override async Task LoadAsync()
        {
            Product[] products = await Task.Run(() => Model.GetProducts().ToArray());
            foreach (Product product in products)
            {
                Items.Insert(Items.Count - 1, new ProductViewModel(product));
            }
        }

        protected override bool CanRepair() => false;

        protected override void Repair()
        {
            // Cannot repair a component.
        }

        protected override bool CanUninstall() => false;

        protected override void Uninstall()
        {
            // Cannot uninstall a component.
        }
    }
}
