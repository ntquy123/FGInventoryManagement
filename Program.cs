using FGInventoryManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    //    WebHost.CreateDefaultBuilder(args)
    //        .UseStartup<Startup>();

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var appSettings = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")));
        var environmentValue = appSettings["Environment"].ToString();
        return Host.CreateDefaultBuilder(args)
            .ConfigureLogging((hostingContext, loggingBuilder) =>
            {
                loggingBuilder.AddConsole(x => x.IncludeScopes = true).AddDebug();
            })
            .UseNLog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                if (!String.IsNullOrEmpty(environmentValue))
                {
                    webBuilder.UseEnvironment(environmentValue);
                }
                //When start multi app, so we need to config it
                //webBuilder.UseUrls(appSettings["Urls"].ToString());
                webBuilder.UseStartup<Startup>();
            });
    }
}