using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;

namespace SystemVariableManager.Managers
{
    public partial class SystemVariableManager
    {
        /// <summary>
        /// Setting group classification
        /// </summary>
        public enum Setting
        {
            Generic,

            // TODO : Add a group if needed.
        }

        private const string appsettings = "appsettings", generic = "generic";

        private static readonly string appsettingsPath = Path.Combine(Environment.CurrentDirectory, $"{appsettings}.json");

        private IConfiguration? configuration;

        public static SystemVariableManager Instance
        {
            get
            {
                if (instance == null || !instance.IsValueCreated)
                    instance = new Lazy<SystemVariableManager>(() => new SystemVariableManager());

                return instance.Value;
            }
        }
        private static Lazy<SystemVariableManager>? instance;

        private SystemVariableManager()
        {
            if (!File.Exists(appsettingsPath))
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(SystemVariableManager)}.{appsettings}.json")) // Create the 'appsettings.json' file as the default file for the resource when it does not exist.
                {
                    if (stream == null)
                        throw new ApplicationException($"'{appsettings}' resource does not exist.");

                    using var fileStream = File.Create(appsettingsPath);
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }

                if (!File.Exists(appsettingsPath))
                    throw new FileNotFoundException($"'{appsettingsPath}' file not found. Not created.");
            }

            LoadConfiguration();
        }

        public void Initialize()
        {
            bool createdSettings = false;

            InitializeProperties(Setting.Generic, generic);

            void InitializeProperties(Setting settingGroup, string root)
            {
                var properties = GetType().GetProperties().Where(p => p.GetCustomAttributes<SettingsAttribute>().Any(a => a.SettingGroup == settingGroup)); // Gets the properties for each group.
                foreach (var property in properties)
                {
                    var settingsValue = GetSettingsValue(root, property.Name);
                    if (settingsValue != null) // If the key value is not empty, handle it as normal.
                        continue;

                    property.SetValue(this, property.GetCustomAttribute<SettingsAttribute>()?.DefaultValue); // Generates a key, value as the default.
                    createdSettings = true;
                }
            }

            if (!createdSettings)
                return;

            System.Diagnostics.Debug.WriteLine($"'{appsettings}.json' is missing or arbitrarily modified or corrupted.");
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile(Path.GetFileName(appsettingsPath), false);
            configuration = configurationBuilder.Build();
        }

        private string? GetGenericSettingsValue([CallerMemberName] string? propertyName = null)
        {
            return GetSettingsValue(generic, propertyName);
        }

        private string? GetSettingsValue(string root, [CallerMemberName] string? propertyName = null)
        {
            return configuration?[$"{root}:{propertyName}"] ?? null;
        }

        private void SetGenericSettingsValue(object? value, [CallerMemberName] string? propertyName = null)
        {
            SetSettingsValue(generic, value, propertyName);
        }

        private void SetSettingsValue(string root, object? value, [CallerMemberName] string? propertyName = null)
        {
            var merge = new JObject { [root] = new JObject() };
            merge[root]![propertyName!] = value?.ToString();

            var settings = JObject.Parse(File.ReadAllText(appsettingsPath, Encoding.UTF8));
            settings.Merge(merge, new JsonMergeSettings { MergeNullValueHandling = MergeNullValueHandling.Merge }); // Merge if there is a key value and generate if there is none.

            File.WriteAllText(appsettingsPath, Newtonsoft.Json.JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented), Encoding.UTF8); // Save first to the local configuration file.

            LoadConfiguration(); // Loads the saved local configuration file.
        }
    }
}
