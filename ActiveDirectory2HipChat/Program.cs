using ActiveDirectory2HipChat.Processors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveDirectory2HipChat
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define the task factory
            var tasks = new List<Task>();

            // Cancellation tokens are a good thing, even if we don't use them all that much
            var cancellationToken = new CancellationTokenSource();

            // Add the tasks
            tasks.Add(Task.Factory.StartNew(() => new AdProcessor().Run(cancellationToken)));
            tasks.Add(Task.Factory.StartNew(() => new HipchatProcessor().RunAsync(cancellationToken)));

            // Fire them all up and wait for them to complete... this shouldn't happen, otherwise the loops are broken.
            Task.WaitAll(tasks.ToArray());

            // Blow it all up
            cancellationToken.Cancel();
        }
    }
}
