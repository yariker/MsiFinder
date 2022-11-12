// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MsiFinder.ViewModel;
using MsiFinder.ViewModel.Messages;
using MvvmMicro;

namespace MsiFinder.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<ShowViewMessage<ProductDetailsViewModel>>(this, msg =>
            {
                var view = new ProductDetailsWindow
                {
                    Owner = this,
                    DataContext = msg.ViewModel,
                };

                view.ShowDialog();
            });
        }

        private void TreeViewItemRightMouseDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = (TreeViewItem)sender;
            treeViewItem.IsSelected = true;
            e.Handled = true;
        }
    }
}
