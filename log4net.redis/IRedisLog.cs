using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace log4net.redis
{
    public interface IRedisLog : ILog
    {
        ILog ObserverKey(string key, ILog log, IRedisConnectionFactory redisConnectionFactory);
    }
}
