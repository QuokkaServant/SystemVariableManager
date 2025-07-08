namespace SystemVariableManagerSample
{
    using Managers;

    public class MainWindowViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        public bool AutomaticRunWindows
        {
            get => SystemVariableManager.Instance.AutomaticRunWindows;
            set
            {
                SystemVariableManager.Instance.AutomaticRunWindows = value;
                OnPropertyChanged();
            }
        }

        public string? Username
        {
            get => SystemVariableManager.Instance.Username;
            set
            {
                SystemVariableManager.Instance.Username = value;
                OnPropertyChanged();
            }
        }
    }
}
