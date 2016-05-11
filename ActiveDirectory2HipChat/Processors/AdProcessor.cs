using ActiveDirectory2HipChat.Directory;
using ActiveDirectory2HipChat.Services;
using System;
using System.Configuration;
using System.Threading;
using ActiveDirectory2HipChat.Data;
using NLog;

namespace ActiveDirectory2HipChat.Processors
{
    public class AdProcessor
    {
        private static readonly Logger Logger = LogManager.GetLogger("Ad2HipChat");
        private readonly int _interval = 60000;

        public AdProcessor()
        {
            var intervalConfig = int.Parse(ConfigurationManager.AppSettings["ad.syncInterval"]);
            Logger.Debug("AD Interval from app.config: {0}", intervalConfig);

            if (intervalConfig > 0) _interval = intervalConfig;
            Logger.Debug("AD Interval: {0}", _interval);
        }

        public void Run(CancellationTokenSource token)
        {
            while (!token.IsCancellationRequested)
            {
                Logger.Debug("AD Processor Running");
                Logger.Trace("Cancellation Requested? " + token.IsCancellationRequested);
                try
                {
                    // TODO: DI this stuff
                    var dir = new DirectoryContext();
                    var ad = new UserService(dir);

                    // Load AD Users into memory
                    var users = ad.GetAllUsers();

                    var db = new DataContext();

                    // Save New Users or Update Existing Users
                    foreach (var user in users)
                    {
                        var usr = new User()
                        {
                            Principal = user.UserPrincipalName,
                            Email = user.EmailAddress,
                            FirstName = user.GivenName,
                            LastName = user.Surname,
                            IsEnabled = user.Enabled ?? false
                        };

                        db.Users.Add(usr);
                    }

                    db.SaveChanges();

                    Logger.Debug("AD Processor Sleeping");
                    Thread.Sleep(_interval);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "AD Processor Failed");

                    Logger.Trace("Calling for Token Cancellation");
                    token.Cancel();
                }
            }
        }
    }
}
