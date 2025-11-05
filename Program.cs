using erpsolution.logger;
using FGInventoryManagement;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Clear default logging providers so NLog is the single source of truth for log output.
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// Instantiate Startup so we can continue using the familiar Configure/ConfigureServices pattern.
var startup = new Startup(builder.Configuration, builder.Environment);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Resolve the logging dependencies that Startup.Configure expects.
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
var loggerManager = app.Services.GetRequiredService<ILoggerManager>();

// Configure the HTTP pipeline using the existing Startup logic.
startup.Configure(app, app.Environment, loggerFactory, app.Services, loggerManager);

app.Run();
