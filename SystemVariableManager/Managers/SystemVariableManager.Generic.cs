using System;
using System.IO;

using IWshRuntimeLibrary;

namespace SystemVariableManager.Managers
{
    using File = System.IO.File;

    public partial class SystemVariableManager
    {
        [Settings(Setting.Generic, false)]
        public bool AutomaticRunWindows
        {
            get => bool.TryParse(GetGenericSettingsValue(), out bool parsed) ? parsed : false;
            set
            {
                var updated = UpdateAutomaticWindowsStartup(value);
                SetGenericSettingsValue(updated);
            }
        }

        // TODO : Add properties if necessary.

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
