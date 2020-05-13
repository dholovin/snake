using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Snake.Common;
using Snake.ServiceContracts.Interfaces;

namespace Snake.ServiceContracts
{
    public class SnakeHostedService : BaseComponent, IHostedService
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
            await _game.Initialize(cancellationToken);

            // ThreadPool.QueueUserWorkItem(async state => {
            await Task.Run(async () => {
                var gameOver = false;
                
                while (!gameOver && !cancellationToken.IsCancellationRequested)
                {
                    await _game.Play(cancellationToken);

                    gameOver = !(await _game.ShouldPlayAgain(cancellationToken));
                }

                
                await _game.Terminate(cancellationToken);

            }, cancellationToken);
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