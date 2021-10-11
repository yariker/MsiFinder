// Copyright (c) Yaroslav Bugaria. All rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using MsiFinder.Model;
using MvvmMicro;

namespace MsiFinder.ViewModel
{
    public abstract class RecordViewModel<T> : RecordViewModel
        where T : Record
    {
        private readonly Lazy<string> _account;
        private TextItemViewModel _textItem;

        protected RecordViewModel(T model, bool loadable = true)
            : this()
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            ExpandCommand = new AsyncRelayCommand(ExpandAsync);
            OpenWindowsExplorerCommand = new RelayCommand(OpenWindowsExplorer, CanOpenWindowsExplorer);
            OpenRegistryEditorCommand = new RelayCommand(OpenRegistryEditor, CanOpenRegistryEditor);
            CopyCodeCommand = new RelayCommand(CopyCode);
            CopyRegistryCodeCommand = new RelayCommand(CopyRegistryCode);
            UninstallCommand = new RelayCommand(Uninstall, CanUninstall);
            RepairCommand = new RelayCommand(Repair, CanRepair);

            if (loadable)
            {
                Items.Add(_textItem = new TextItemViewModel());
            }
        }

        private RecordViewModel()
        {
            _account = new Lazy<string>(GetAccount);
        }

        public T Model { get; }

        public string Account => _account.Value;

        public abstract string Location { get; }

        public override ICommand ExpandCommand { get; }

        public override ICommand OpenWindowsExplorerCommand { get; }

        public override ICommand OpenRegistryEditorCommand { get; }

        public override ICommand CopyCodeCommand { get; }

        public override ICommand CopyRegistryCodeCommand { get; }

        public override ICommand RepairCommand { get; }

        public override ICommand UninstallCommand { get; }

        protected virtual Task LoadAsync() => Task.CompletedTask;

        protected abstract bool CanUninstall();

        protected abstract void Uninstall();

        protected abstract bool CanRepair();

        protected abstract void Repair();

        private async Task ExpandAsync()
        {
            if (_textItem != null)
            {
                _textItem.Text = "Loading...";
                _textItem.Type = TextItemType.Information;

                try
                {
                    await LoadAsync();
                }
                catch (Exception ex)
                {
                    _textItem.Text = ex.Message;
                    _textItem.Type = TextItemType.Error;
                    return;
                }

                Items.Remove(_textItem);
                _textItem = null;
            }
        }

        private bool CanOpenWindowsExplorer() => Directory.Exists(Location) || File.Exists(Location);

        private void OpenWindowsExplorer()
        {
            try
            {
                if (Directory.Exists(Location))
                {
                    Process.Start(new ProcessStartInfo("explorer", Location) { UseShellExecute = true, });
                }
                else if (File.Exists(Location))
                {
                    Process.Start(new ProcessStartInfo("explorer", $"/select,\"{Location}\"")
                    {
                        UseShellExecute = true,
                    });
                }
            }
            catch (Win32Exception)
            {
                // Don't care.
            }
        }

        private bool CanOpenRegistryEditor() => Model.CheckExists();

        private void OpenRegistryEditor()
        {
            Registry.SetValue(
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit",
                "Lastkey",
                Model.RegistryKey);

            try
            {
                Process.Start(new ProcessStartInfo("regedit") { UseShellExecute = true });
            }
            catch (Win32Exception)
            {
                // Don't care.
            }
        }

        private string GetAccount()
        {
            var sid = new SecurityIdentifier(Model.Sid ?? Record.SystemSid);
            string account = sid.Translate(typeof(NTAccount)).Value;
            return account;
        }

        private void CopyCode()
        {
            Clipboard.SetText(Model.Code.ToString("B").ToUpper());
        }

        private void CopyRegistryCode()
        {
            Clipboard.SetText(Model.PackedCode);
        }
    }
}
