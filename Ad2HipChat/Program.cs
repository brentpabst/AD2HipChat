using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Ad2HipChat.Data;
using Ad2HipChat.Migrations;
using Ad2HipChat.Processors;
using Ad2HipChat.Services;
using Ninject;
using NLog;

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

            Logger.Trace("Adding AD Processor");
            tasks.Add(Task.Factory.StartNew(() => new AdProcessor(userService, userRepository).Run(cancellationToken), cancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default));

            Logger.Trace("Adding HipChat Processor");
            tasks.Add(Task.Factory.StartNew(() => new HipchatProcessor().Run(cancellationToken), cancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default));

            // Fire them all up and wait for them to complete... this shouldn't happen, otherwise the loops are broken.
            Logger.Trace("Running the task factory... firing the missles");
            Task.WaitAll(tasks.ToArray());

            Logger.Info("Stopping Ad2HipChat Integration");

            Logger.Trace("Cancelling all running cooperative tasks");
            cancellationToken.Cancel();

            Logger.Info("Ad2HipChat Integration Stopped");
        }
    }
}
