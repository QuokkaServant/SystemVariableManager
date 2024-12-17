namespace SystemVariableManager
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
    }
}
