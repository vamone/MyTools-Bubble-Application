using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyTools.Desktop.App.Models;
using MyTools.Desktop.App.Services;
using MyTools.Desktop.App.Utilities;
using MyTools.Desktop.App2;
using MyTools.Desktop.App2.ConfigOptions;
using MyTools.Desktop.App2.Managers;
using MyTools.Desktop.App2.Models;
using MyTools.Desktop.App2.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MyTools.Desktop.App;

public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    readonly ServiceProvider _serviceProvider;

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<MainWindow>();
                services.AddSingleton<LoggerWindow>();

                services.AddTransient<IFileReaderService<ICollection<string>>, DataFileReaderService>();
                services.AddTransient<IFileReaderService<Settings>, SettingsFileReaderService>();
                services.AddTransient<IFileReaderService<ICollection<TaskLogger>>, TaskLoggerService>();

                services.AddSingleton<IStackConfig<IFocusElement>>(x => new StackConfig<IFocusElement>(x.GetRequiredService<IFileReaderService<Settings>>())
                {
                    ForegroundColor = Brushes.White,
                    BackgroundColor = Brushes.Red,
                    BorderBrush = () => Brushes.Red,
                    ClickAction = (s, e) => x.GetRequiredService<MainWindow>().StartFocusTimerWithLogger(s, e),
                    FuncFindResource = (a) => x.GetRequiredService<MainWindow>().FindResource(a),
                });

                services.AddTransient<WorkAreaManager>(x => new WorkAreaManager(x.GetRequiredService<IFileReaderService<Settings>>(), (a) => x.GetRequiredService<MainWindow>().FindResource(a)));
                services.AddTransient<ClipboardsManager>();

                services.Configure<DataFileConfig>(options => hostContext.Configuration.GetSection(DataFileConfig.ConfigBinding).Bind(options));
                services.Configure<SettingsFileConfig>(options => hostContext.Configuration.GetSection(SettingsFileConfig.ConfigBinding).Bind(options));

            }).Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        AppHost.Services.GetRequiredService<MainWindow>()?.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();
        base.OnExit(e); 
    }
}