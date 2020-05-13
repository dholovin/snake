using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Snake.Common;
using Snake.ServiceContracts.Interfaces;

namespace Snake.ServiceContracts
{
    public class SnakeHostedService : BaseService, IHostedService
    {
        private IInputOutputService _inputOutputService;
        private IGame _game;

        public SnakeHostedService(IInputOutputService io, IGame game)// : base(io)
        {
            _inputOutputService = io;
            _game = game;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            // TODO: check difference between Exception Handling
            // Task.Run(async () =>                        // .NET 4.5
            // ThreadPool.QueueUserWorkItem(async state => // .NET 1.1
            try
            {
                var exitGame = false;
                
                while (!exitGame && !cancellationToken.IsCancellationRequested)
                {
                    var (initialLevel, screenSizeMultiplier) = await _game.AskForInitialSetup(cancellationToken);
                    // await _game.Initialize(initialLevel, screenSizeMultiplier, cancellationToken);
                    var gameStatus = await _game.Play(initialLevel, screenSizeMultiplier, cancellationToken);

                    exitGame = !(await _game.ShouldPlayAgain(cancellationToken));
                }
                
                await _game.Terminate(cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                 // Expected after the worker performs:
                // StopAsync(cancellationToken);
                // cancellationToken.ThrowIfCancellationRequested();
                // TODO: how to handle in non-terminal scenarios?
                System.Terminal.OutLine(ex.Message);
                System.Terminal.ReadLine();
            }
            // }, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            var msg  = "Thank you for playing. Good bye.";
            await _inputOutputService.Clear(cancellationToken);
            await _inputOutputService.Print(0, 0, msg, cancellationToken);
            // await Task.Delay(2000); // would throw exception to host.RunTerminalAsync()
            Thread.Sleep(2000);
        }
    }
}