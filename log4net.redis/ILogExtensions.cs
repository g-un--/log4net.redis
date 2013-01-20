using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace log4net.redis
{
    static class ILogExtensions
    {
        public static ILog ObserverKey(this ILog log, string key)
        {
            return new RedisLogger(key, log, new RedisConnectionFactory());
        }
    }
}
