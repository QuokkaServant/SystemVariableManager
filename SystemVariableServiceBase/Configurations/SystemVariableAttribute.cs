namespace SystemVariableService.Configurations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class SystemVariableAttribute(string category, object? defaultValue, string? key = null) : Attribute
    {
        public string Category { get; init; } = category;

        public object? DefaultValue { get; init; } = defaultValue;

        public string? Key { get; init; } = key;
    }
}
