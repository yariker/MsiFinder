using Autofac;

namespace MsiFinder.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();
            Container = builder.Build();
        }

        private IContainer Container { get; }

        public MainViewModel Main => Container.Resolve<MainViewModel>();
    }
}