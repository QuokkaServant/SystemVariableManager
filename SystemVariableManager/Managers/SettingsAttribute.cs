namespace SystemVariableManager.Managers
{
    using Setting = SystemVariableManager.Setting;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class SettingsAttribute : Attribute
    {
        /// <summary>
        /// Divide the group that contains the setting values.
        /// </summary>
        public Setting SettingGroup { init; get; }

        /// <summary>
        /// Sets the default value when no value is set.
        /// </summary>
        public object? DefaultValue { init; get; }

        public SettingsAttribute(Setting settingGroup, object? defaultValue)
        {
            SettingGroup = settingGroup;
            DefaultValue = defaultValue;
        }
    }
}
