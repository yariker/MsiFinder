using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MsiFinder.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TreeViewItemRightMouseDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = (TreeViewItem)sender;
            treeViewItem.IsSelected = true;
            e.Handled = true;
        }
    }
}
