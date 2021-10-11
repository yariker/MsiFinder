// Copyright (c) Yaroslav Bugaria. All rights reserved.

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

        public MainViewModel Main => Container.Resolve<MainViewModel>();

        private IContainer Container { get; }
    }
}
