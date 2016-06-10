using System.Configuration;
using System.DirectoryServices.AccountManagement;

namespace Ad2HipChat.Directory
{
    public class DirectoryContext : IDirectoryContext
    {
        private readonly DirectorySettings _settings;

        public DirectoryContext()
        {
            _settings = new DirectorySettings
            {
                Directory = ConfigurationManager.AppSettings["ad.directory"],
                Container = ConfigurationManager.AppSettings["ad.container"],
                Username = ConfigurationManager.AppSettings["ad.username"],
                Password = ConfigurationManager.AppSettings["ad.password"]
            };
        }

        public PrincipalContext LoadAndConnect()
        {
            return new PrincipalContext(ContextType.Domain,
                _settings.Directory,
                _settings.Container,
                ContextOptions.Negotiate,
                _settings.Username,
                _settings.Password);
        }
    }
}
