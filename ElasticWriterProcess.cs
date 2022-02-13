using Nest;
using Newtonsoft.Json;
using Plain.RabbitMQ;

namespace LogSaverService
{
    public class ElasticWriterProcess : IHostedService
    {
        private readonly ISubscriber _subscriber;
        private readonly IElasticClient _ec;


        public ElasticWriterProcess(ISubscriber subscriber, IElasticClient ec)
        {
            _subscriber = subscriber;
            _ec = ec;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(ProcessMessageFromRMQ);
            return Task.CompletedTask;
        }

        private bool ProcessMessageFromRMQ(string message, IDictionary<string, object> headers)
        {
            Console.WriteLine(message);

            var res = _ec.Index<Log>(new Log() { datetime = DateTime.Now, message = message }, x => x.Index("transactions"));
            while(!res.ApiCall.Success)
            {
                res = _ec.Index<Log>(new Log() { datetime = DateTime.Now, message = message }, x => x.Index("transactions"));
                Task.Delay(1000).Wait();
                Console.WriteLine("sabotage");
            }
            Console.WriteLine("sabotage");
            return true;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class Log
    {
        public DateTime datetime;
        public String message;

    }
}
