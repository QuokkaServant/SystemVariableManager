using System.IO;
using System.Runtime.CompilerServices;
using SystemVariableService.Configurations;

using IWshRuntimeLibrary;

namespace SystemVariableManagerSample.Managers
{
    using File = System.IO.File;

    public partial class SystemVariableManager
    {
        private const string UserCategory = "User";

        [SystemVariable(GenericCategory, false)]
        public bool AutomaticRunWindows
        {
            get => bool.TryParse(GetGenericValue(), out bool parsed) && parsed;
            set
            {
                var updated = UpdateAutomaticWindowsStartup(value);
                SetGenericValue(updated);
            }
        }

        [SystemVariable(UserCategory, "QuokkaServant", "유저")]
        public string? Username
        {
            get => Decryption(GetValue(UserCategory));
            set => SetValue(UserCategory, Encryption(value));
        }

        // TODO : Add properties if necessary.

        private string? GetGenericValue([CallerMemberName] string? propertyName = null)
        {
            return GetValue(GenericCategory, propertyName);
        }

        private void SetGenericValue(object? value, [CallerMemberName] string? propertyName = null)
        {
            SetValue(GenericCategory, value, propertyName);
        }

        private bool UpdateAutomaticWindowsStartup(bool startup)
        {
            try
            {
                string startupDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup)), shortcutPath = Path.Combine(startupDirectory, $"{nameof(SystemVariableManager)}.lnk");
                if (File.Exists(shortcutPath))
                    File.Delete(shortcutPath);

                if (!startup)
                    return false;

                var shortcut = (IWshShortcut)new WshShell().CreateShortcut(shortcutPath);
                shortcut.TargetPath = Environment.ProcessPath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(Environment.ProcessPath);
                shortcut.Save(); // Create a shortcut in the startup program.
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"An exception has occurred update automatic windows {startup} startup. {ex.StackTrace} >> {ex.Message}");
                return false;
            }
        }
    }
}
