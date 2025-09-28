using WebApplicationFirewall.Helpers;
using WebApplicationFirewall.Middleware;
internal class Program
{
    private static void Main(string[] args)
    {
        var ipStorage = new StorageHelper();
        var loadedIps = ipStorage.LoadIps();
        Console.WriteLine("Loaded IPs:");

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<IStorageHelper, StorageHelper>();
        builder.Services.AddSingleton<IFeatureActivatorService, FeatureActivatorService>();
        var app = builder.Build();

        app.MapReverseProxy();
        app.UseMiddleware<BlockedIps>();
        app.UseMiddleware<RateLimit>();
        app.UseMiddleware<SqlInjectionDetection>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}