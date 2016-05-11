using System;
using System.Configuration;
using System.Threading;

namespace ActiveDirectory2HipChat.Processors
{
    public class HipchatProcessor
    {
        private readonly int _interval = 60000;

        public HipchatProcessor()
        {
            var intervalConfig = int.Parse(ConfigurationManager.AppSettings["hipchat.syncInterval"]);
            if (intervalConfig > 0) _interval = intervalConfig;
        }

        public void Run(CancellationTokenSource token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    // Start HipChat Sync Processor
                    // Load Users Who Changed
                    // Call HipChat And Make Changes
                    // Mark Record As Processed

                    Console.WriteLine("HipChat Processor - Sleeping");
                    Thread.Sleep(_interval);
                }
                catch (Exception ex)
                {
                    // Shit blew up, stop the app
                    Console.WriteLine("HipChat Processor - Failed: " + ex.Message);
                    token.Cancel();
                }
            }
        }
    }
}
