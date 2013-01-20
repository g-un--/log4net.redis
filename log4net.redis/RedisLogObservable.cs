using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Reactive.Disposables;

namespace log4net.redis
{
    public static class RedisLogObservable
    {
        public static IObservable<string> CreateObservable(string key, IScheduler scheduler)
        {
            var observable = Observable.Defer(() =>
                {
                    return Observable.Create<string>(observer =>
                   {
                       var connectionFactory = new RedisConnectionFactory();

                       return connectionFactory.CreateRedisConnection().ContinueWith<IDisposable>(task =>
                           {
                               if (task.IsFaulted)
                               {
                                   observer.OnError(task.Exception);
                                   return Disposable.Empty;
                               }

                               return scheduler.Schedule(() =>
                               {
                                   var connection = task.Result.GetOpenSubscriberChannel();
                                   task.Result.Error += (_, err) => observer.OnError(err.Exception);

                                   connection.Subscribe(key, (header, content) =>
                                   {
                                       if (header == key)
                                           observer.OnNext(Encoding.UTF8.GetString(content));
                                   });
                               });
                           });
                   });
                });

             return observable;
        }

        public static IObservable<string> CreateObservableWithRetry(string key, IScheduler scheduler)
        {
            return Observable.Create<string>(observer =>
                {
                    return scheduler.Schedule(self =>
                        {
                            var observable = CreateObservable(key, scheduler);
                            var disposable = observable.Subscribe(
                                message =>
                                {
                                    observer.OnNext(message);
                                }, 
                                error =>
                                {
                                    scheduler.Schedule(TimeSpan.FromSeconds(10), self);
                                });
                        });
                });
        }
    }
}
