using System.Windows.Input;

namespace MsiFinder.ViewModel
{
    public abstract class RecordViewModel : ItemViewModel
    {
        public abstract ICommand ExpandCommand { get; }

        public abstract ICommand OpenWindowsExplorerCommand { get; }

        public abstract ICommand OpenRegistryEditorCommand { get; }

        public abstract ICommand CopyCodeCommand { get; }

        public abstract ICommand CopyRegistryCodeCommand { get; }
    }
}