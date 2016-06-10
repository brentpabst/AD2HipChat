using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Ad2HipChat.Data;
using Ad2HipChat.Processors;
using Ad2HipChat.Services;
using Ninject;
using NLog;
using Configuration = Ad2HipChat.Migrations.Configuration;

namespace Ad2HipChat
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetLogger("Ad2HipChat");

        static void Main(string[] args)
        {
            // Start Logging
            LoggerConfig.StartLogging();

            Logger.Info("Starting Ad2HipChat Integration");

            Logger.Debug("Performing database migration");
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
            Logger.Debug("Database migration complete");

            // Handle Dependency Injection
            IKernel kernel = new StandardKernel(new NinjectConfig());
            var userRepository = kernel.Get<IUserRepository>();
            var userService = kernel.Get<IUserService>();

            // Run!!!

            Logger.Trace("Building task factory");
            var tasks = new List<Task>();

            Logger.Trace("Defining new cancellation token for all processors");
            var cancellationToken = new CancellationTokenSource();

            if (bool.Parse(ConfigurationManager.AppSettings["hipchat.enableSync"]))
            {
                Logger.Warn("Users will be sync'd with HipChat!");
                tasks.Add(new HipchatProcessor(userRepository).Run(cancellationToken));
            }
            else Logger.Warn("HipChat Sync is disabled by configuration!");
            tasks.Add(new AdProcessor(userService, userRepository).Run(cancellationToken));

            Logger.Trace("Running the task factory... firing the missles");
            try
            {
                // Fire them all up and wait for them to complete... this shouldn't happen, otherwise the loops are broken.
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
            }
            finally
            {
                Logger.Info("Stopping Ad2HipChat Integration");

                Logger.Trace("Cancelling all running cooperative tasks");
                cancellationToken.Cancel();
            }

            Logger.Info("Ad2HipChat Integration Stopped");
        }
    }
}
