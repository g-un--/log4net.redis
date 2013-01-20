using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BookSleeve;
using System.Threading.Tasks;

namespace log4net.redis
{
    public interface IRedisConnectionFactory
    {
        Task<RedisConnection> CreateRedisConnection();
    }
}
