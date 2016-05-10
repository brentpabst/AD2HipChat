using ActiveDirectory2HipChat.Directory;
using ActiveDirectory2HipChat.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveDirectory2HipChat.Processors
{
    public class AdProcessor
    {
        private int interval = 60000;

        public AdProcessor()
        {
            var intervalConfig = int.Parse(ConfigurationManager.AppSettings["ad.syncInterval"]);
            if (intervalConfig > 0) interval = intervalConfig;
        }

        public async Task RunAsync(CancellationTokenSource cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // TODO: DI this stuff
                    var dir = new DirectoryContext();
                    var ad = new UserService(dir);

                    // Load AD Users into memory
                    var users = await ad.GetAllUsers();

                    // Save New Users or Update Existing Users
                    foreach (var user in users)
                    {
                        Console.WriteLine(user.DisplayName);
                    }

                    Console.WriteLine("AD Processor - Sleeping");
                    Thread.Sleep(interval);
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
