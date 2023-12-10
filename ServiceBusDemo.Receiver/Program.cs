using ServiceBusDemo.Receiver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBootstrapper(builder.Configuration);

var app = builder.Build();

app.AddAppBootstrapper();

app.Run();