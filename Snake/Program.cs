﻿using System;
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

            var host = TerminalHost
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                    {
                        services.AddSingleton<IHostedService, SnakeHostedService>();
                        services.AddTransient<IGame, SnakeGame>();
                        services.AddTransient<IInputOutputService, TerminalInputOutputService>();
                        services.AddTransient<IFigureService, FigureService>();
                        services.AddTransient<MainView>();
                        services.AddTransient<HelpView>();
                        services.AddTransient<SummaryView>();
                    }
                );

            // try 
            // {
                await host.RunTerminalAsync(options =>
                    {
                        options.Title = "Snake";
                        options.SuppressStatusMessages = true;
                    }, cancellationTokenSource.Token);

            // }
            // catch (OperationCanceledException ex) 
            // {
            //     // Expected after the worker performs:
            //     // StopAsync(cancellationToken);
            //     // cancellationToken.ThrowIfCancellationRequested();
            //     // TODO: how to handle in non-terminal scenarios?
            //     System.Terminal.OutLine(ex.Message);
            //     System.Terminal.ReadLine();
            // } 
            // catch (Exception ex) 
            // {
            //     // TODO: how to handle in non-terminal scenarios?
            //     System.Terminal.OutLine(ex.Message);
            //     System.Terminal.ReadLine();
            // } 

            return 0;
        }
    }
}
