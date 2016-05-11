using ActiveDirectory2HipChat.Directory;
using ActiveDirectory2HipChat.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActiveDirectory2HipChat.Data;

namespace ActiveDirectory2HipChat.Processors
{
    public class AdProcessor
    {
        private readonly int _interval = 60000;

        public AdProcessor()
        {
            var intervalConfig = int.Parse(ConfigurationManager.AppSettings["ad.syncInterval"]);
            if (intervalConfig > 0) _interval = intervalConfig;
        }

        public void Run(CancellationTokenSource cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
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

                        Console.WriteLine(user.DisplayName);
                    }

                    db.SaveChanges();

                    Console.WriteLine("AD Processor - Sleeping");
                    Thread.Sleep(_interval);
                }
                catch (Exception ex)
                {
                    // Shit blew up, stop the app
                    Console.WriteLine("AD Processor - Failed: " + ex.Message);
                    cancellationToken.Cancel();
                }
            }
        }
    }
}
