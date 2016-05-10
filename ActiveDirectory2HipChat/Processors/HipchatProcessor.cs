using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveDirectory2HipChat.Processors
{
    public class HipchatProcessor
    {
        private int interval = 60000;

        public HipchatProcessor()
        {
            var intervalConfig = int.Parse(ConfigurationManager.AppSettings["hipchat.syncInterval"]);
            if (intervalConfig > 0) interval = intervalConfig;
        }

        public async Task RunAsync(CancellationTokenSource cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Start HipChat Sync Processor
                // Load Users Who Changed
                // Call HipChat And Make Changes
                // Mark Record As Processed

                Console.WriteLine("HipChat Processor - Sleeping");
                Thread.Sleep(interval);
            }
        }
    }
}
