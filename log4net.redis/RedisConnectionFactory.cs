using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookSleeve;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace log4net.redis
{
    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        public Task<RedisConnection> CreateRedisConnection()
        {
            var settings = RedisConnectionProvider.Instance.ConnectionsSettings;
            var redisConnection = new RedisConnection(host: settings["host"], port: Convert.ToInt32(settings["port"]), password: settings["password"]);
            var taskCreateConnection = new TaskCompletionSource<RedisConnection>();

            try
            {
                redisConnection.Open().ContinueWith((task) =>
                    {
                        if (!task.IsFaulted)
                        {
                            taskCreateConnection.SetResult(redisConnection);
                        }
                        else
                        {
                            task.Exception.Handle(x => true);
                            taskCreateConnection.SetException(task.Exception);
                        }
                    }, TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                taskCreateConnection.SetException(ex);
            }

            return taskCreateConnection.Task;
        }
    }
}
