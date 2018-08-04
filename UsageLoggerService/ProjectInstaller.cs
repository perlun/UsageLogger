using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace UsageLoggerService
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            serviceProcessInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Password = null;
            serviceProcessInstaller.Username = null;

            serviceInstaller.Description = 
                "UsageLoggerService. This service logs computer usage of the Windows workstation to an SQL database";
            serviceInstaller.DisplayName = "Usage Logger";
            serviceInstaller.ServiceName = "usage_logger";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            Installers.AddRange(new Installer[] {
                serviceProcessInstaller,
                serviceInstaller
            });
        }
    }
}
