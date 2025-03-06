
using Microsoft.VisualBasic;
using StackExchange.Redis;
using System.Collections.Concurrent;


namespace FileUpload.Controllers
{
    public class BackgroundObserver : IHostedService, IDisposable
    {
        ConnectionMultiplexer redis;
        IDatabase db;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly ConcurrentBag<Task> _workerTasks = new();
        private uint taskNumber = 4;
        public BackgroundObserver()
        {
             redis = ConnectionMultiplexer.Connect("file.database:6379");
             db = redis.GetDatabase();
            for (int i = 0; i < taskNumber; i++)
            {
                var workerTask = Task.Run(() => ProcessFiles(_cancellationTokenSource.Token));
                _workerTasks.Add(workerTask);
            }
        }


        private async Task ProcessFiles(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var fileId = await db.ListLeftPopAsync("filesQueueReady");
                    if (fileId.IsNullOrEmpty)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1), token);
                        continue;
                    }
                    var content = await db.StringGetAsync($"fileContent:{fileId}");
                    if (content.IsNullOrEmpty)
                    {
                        continue;
                    }
                    var reversedContent = shorten(content.ToString());

                    await db.StringSetAsync($"fileContent:{fileId}", reversedContent);

                    await db.ListRightPushAsync("filesQueueDone", fileId);
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while working on file: {ex.Message}");
                }
            }
        }

        private string shorten(string value)
        {
            string[] splitted = value.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ",splitted.Take(10));
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            redis.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}
