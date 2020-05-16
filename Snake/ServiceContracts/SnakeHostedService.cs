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
            // Task.Run(async () => {                       // .NET 4.5
            // ThreadPool.QueueUserWorkItem(async state => {// .NET 1.1
            try
            {
                var playAgain = true;

                while (playAgain && !cancellationToken.IsCancellationRequested)
                {
                    var (initialSpeed, screenSizeMultiplier) = await _game.AskForInitialSetup(cancellationToken);
                    await _game.Initialize(screenSizeMultiplier, cancellationToken);
                    var gameStatus = await _game.Play(initialSpeed, cancellationToken);
                    playAgain = await _game.ShouldPlayAgain(cancellationToken);
                }
                
                await _game.Terminate(cancellationToken);
            }
            catch (OperationCanceledException ex) // handling cancellation
            {
                // Expected after the worker performs:
                // StopAsync(cancellationToken);
                // cancellationToken.ThrowIfCancellationRequested();
                // await _inputOutputService.Print(0, 0, ex.Message, cancellationToken);
                // await _inputOutputService.GetString(cancellationToken);
                await _inputOutputService.Terminate(cancellationToken);
            }
            catch (Exception ex)
            {
                // await _inputOutputService.Print(0, 0, ex.Message, cancellationToken);
                // await _inputOutputService.GetString(cancellationToken);
                await _inputOutputService.Terminate(cancellationToken);
            }
            // }, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await _inputOutputService.Clear(cancellationToken);
            await _inputOutputService.Print(0, Constants.MAIN_SCREEN_MIN_HEIGHT + Constants.SUMMARY_PANE_HEIGHT,
                Constants.GameCopyright, cancellationToken);
            await Task.Delay(2000);
        }
    }
}