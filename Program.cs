using LogSaverService;
using Nest;
using Plain.RabbitMQ;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@192.168.5.3:5672"));
builder.Services.AddSingleton<Plain.RabbitMQ.ISubscriber>(
    x => new Subscriber(x.GetService<IConnectionProvider>(),
    "transfer_log_exchange",
    "transfer_queue",
    "transfer.*",
    ExchangeType.Topic));

var settings = new ConnectionSettings(new Uri(@"http://192.168.5.3:9200"));
builder.Services.AddSingleton<IElasticClient>(new ElasticClient(settings));

builder.Services.AddHostedService<ElasticWriterProcess>();
var app = builder.Build();



app.Run();
