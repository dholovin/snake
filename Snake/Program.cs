using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Snake.ServiceContracts;
using Snake.ServiceContracts.Interfaces;
using Snake.Views;

namespace Snake
{
    class Program
    {
        private static async Task<int> Main(string[] args)
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            await TerminalHost
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                    {
                        services.AddSingleton<IHostedService, SnakeHostedService>();
                        services.AddTransient<IGame, SnakeGame>();
                        services.AddTransient<IInputOutputService, TerminalInputOutputService>();
                        services.AddTransient<IFigureService, FigureService>();
                        services.AddTransient<MainView>();
                        services.AddTransient<SummaryView>();
                        services.AddTransient<HelpView>();
                    }
                ).RunTerminalAsync(options =>
                    {
                        options.Title = "Snake";
                        options.SuppressStatusMessages = true;
                    }, cancellationTokenSource.Token);

            return 0;
        }
    }
}
