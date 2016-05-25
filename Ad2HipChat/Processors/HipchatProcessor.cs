using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using Ad2HipChat.Data;
using NLog;

namespace Ad2HipChat.Processors
{
    public class HipchatProcessor
    {
        private static readonly Logger Logger = LogManager.GetLogger("Ad2HipChat.HipChatProcessor");
        private readonly int _interval = 60000;
        private readonly IUserRepository _userRepository;

        public HipchatProcessor(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            var intervalConfig = int.Parse(ConfigurationManager.AppSettings["hipchat.syncInterval"]);
            Logger.Debug("HipChat Interval from app.config: {0}", intervalConfig);

            if (intervalConfig > 0) _interval = intervalConfig;
            Logger.Debug("HipChat Interval: {0}", _interval);
        }

        public void Run(CancellationTokenSource token)
        {
            while (!token.IsCancellationRequested)
            {
                Logger.Debug("HipChat Processor Running");
                Logger.Trace("Cancellation Requested? " + token.IsCancellationRequested);
                try
                {
                    Logger.Debug("Loading DB Users to Sync");

                    var users = _userRepository.All().ToList();

                    Logger.Debug("Found {0} users", users.Count);

                    if (users.Count == 0) Logger.Trace("No users to sync, time to sleep");

                    foreach (var user in users)
                    {
                        Logger.Trace("Syncing " + user.Principal);

                        // Call HipChat And Make Changes



                        Logger.Trace(user.Principal + "is synced with HipChat, storing results...");

                        user.IsSynced = true;
                        user.SyncedOn = DateTime.UtcNow;

                        Logger.Trace("Saving user changes");
                        _userRepository.Save(user);

                        Logger.Trace("Committing user changes.");
                        _userRepository.Commit();

                        Logger.Trace("Done with " + user.Principal);
                    }

                    Logger.Debug("HipChat Processor Sleeping");
                    Thread.Sleep(_interval);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "HipChat Processor Failed");

                    Logger.Trace("Calling for Token Cancellation");
                    token.Cancel();
                }
            }
        }
    }
}
