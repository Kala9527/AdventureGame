using System.Windows;
using Microsoft.Extensions.Configuration;

namespace AdventureGame;

public partial class App : Application
{
    public static IConfiguration Configuration { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.example.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        DispatcherUnhandledException += (s, args) =>
        {
            System.IO.File.AppendAllText("error.log", $"[{DateTime.Now}] {args.Exception}\n");
            args.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            System.IO.File.AppendAllText("error.log", $"[{DateTime.Now}] {(Exception)args.ExceptionObject}\n");
        };

        base.OnStartup(e);
    }
}
