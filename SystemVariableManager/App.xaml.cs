using System.Windows;

namespace SystemVariableManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Managers.SystemVariableManager.Instance.Initialize();
            base.OnStartup(e);
        }
    }
}
