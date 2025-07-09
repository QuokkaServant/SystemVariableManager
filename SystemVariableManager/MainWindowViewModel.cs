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

        public System.Windows.Input.ICommand ResetConfigurationsCommand { get; }

        public MainWindowViewModel()
        {
            ResetConfigurationsCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(ResetConfigurations);
        }

        private void ResetConfigurations()
        {
            SystemVariableManager.Instance.Reset();
            OnPropertyChanged(nameof(AutomaticRunWindows));
            OnPropertyChanged(nameof(Username));
        }
    }
}
