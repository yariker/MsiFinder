using System;
using System.Linq;
using System.Threading.Tasks;
using MsiFinder.Model;

namespace MsiFinder.ViewModel
{
    public class ProductViewModel : RecordViewModel<Product>
    {
        public ProductViewModel(Product model)
            : base(model)
        {
        }

        public override string Location => Model.Location;

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
    }
}