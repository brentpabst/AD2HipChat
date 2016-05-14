using System;
using System.Configuration;
using System.Threading;
using NLog;

namespace Ad2HipChat.Processors
{
    public class HipchatProcessor
    {
        private static readonly Logger Logger = LogManager.GetLogger("Ad2HipChat.HipChatProcessor");
        private readonly int _interval = 60000;

        public HipchatProcessor()
        {
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
                    // Start HipChat Sync Processor
                    // Load Users Who Changed
                    // Call HipChat And Make Changes
                    // Mark Record As Processed

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
