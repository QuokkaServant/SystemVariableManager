using System.IO;

namespace SystemVariableManagerSample.Managers
{
    public partial class SystemVariableManager : SystemVariableService.Configurations.SystemVariableServiceBase
    {
        private static readonly string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(SystemVariableManager));

        private static readonly Lazy<SystemVariableManager> instance = new(() => new SystemVariableManager());

        protected override string SettingsPath => Path.Combine(directory, $"Manager-{Filename}");

        public static SystemVariableManager Instance => instance.Value;

        private SystemVariableManager() : base() { }

        static SystemVariableManager()
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}
