using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;

namespace SystemVariableService.Configurations
{
    using Utils;

    public abstract partial class SystemVariableServiceBase
    {
        protected const string Filename = @"settings.json", GenericCategory = "Generic";

        private readonly object lockObject = new();

        private IConfiguration? configuration;

        /// <summary>
        /// Use override to change the file path.
        /// </summary>
        protected virtual string SettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), nameof(SystemVariableServiceBase), Filename);

        protected SystemVariableServiceBase()
        {
            lock (lockObject)
            {
                if (!File.Exists(SettingsPath))
                    CreateDefaultFileResource(SettingsPath);
            }
            LoadConfiguration();
            Initialize();
        }

        private void CreateDefaultFileResource(string path)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(SystemVariableService)}.{Filename}") ?? throw new ApplicationException($"'{Filename}' resource does not exist.");
            using var fileStream = File.Create(path);
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(fileStream);

            if (!File.Exists(path)) throw new FileNotFoundException($"'{path}' file not found. Not created.");
        }

        private void Initialize()
        {
            var properties = GetType().GetProperties().Where(p => p.IsDefined(typeof(SystemVariableAttribute), true));
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<SystemVariableAttribute>()!;
                var settingValue = GetValue(attribute.Category, property.Name);
                if (settingValue != null)
                    continue;

                property.SetValue(this, attribute.DefaultValue);
            }
        }

        private void LoadConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(SettingsPath)!).AddJsonFile(Path.GetFileName(SettingsPath), false);
            configuration = configurationBuilder.Build();
        }

        private string GetKeyname(string propertyName)
        {
            return GetType().GetProperty(propertyName)?.GetCustomAttribute<SystemVariableAttribute>()?.Key ?? propertyName;
        }

        protected string? GetValue(string category, [CallerMemberName] string? propertyName = null)
        {
            return configuration?[$"{category}:{GetKeyname(propertyName)}"] ?? null;
        }

        protected void SetValue(string category, object? value, [CallerMemberName] string? propertyName = null)
        {
            lock (lockObject)
            {
                var merge = new JObject { [category] = new JObject() };
                merge[category]![GetKeyname(propertyName)] = value?.ToString();

                var settings = JObject.Parse(File.ReadAllText(SettingsPath));
                settings.Merge(merge, new JsonMergeSettings { MergeNullValueHandling = MergeNullValueHandling.Merge });

                File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented), Encoding.UTF8);

                LoadConfiguration();
            }
        }

        public virtual void Reset()
        {
            lock (lockObject)
            {
                if (File.Exists(SettingsPath))
                    File.Delete(SettingsPath);

                CreateDefaultFileResource(SettingsPath);
            }
            LoadConfiguration();
            Initialize();
        }

        protected static string? Encryption(string? input, byte[]? optionalEntropy = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input) || DataProtectionApi.Encryption(Encoding.UTF8.GetBytes(input), optionalEntropy) is not byte[] encoding)
                    return null;

                return Convert.ToBase64String(encoding);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to encrypt '{input}' character. {ex.StackTrace} >> {ex.Message}");
                return null;
            }
        }

        protected static string? Decryption(string? input, byte[]? optionalEntropy = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input) || DataProtectionApi.Decryption(Convert.FromBase64String(input), optionalEntropy) is not byte[] decoding)
                    return null;

                return Encoding.UTF8.GetString(decoding);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to decrypt '{input}' character. {ex.StackTrace} >> {ex.Message}");
                return null;
            }
        }
    }
}
