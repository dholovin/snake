using System.Threading;
using System.Threading.Tasks;
using Snake.Models;

namespace Snake.ServiceContracts.Interfaces
{
    public interface IGame
    {
        public Task<(short, short)> AskForInitialSetup(CancellationToken cancellationToken);
        public Task Initialize(short screenSizeMultiplier, CancellationToken cancellationToken);
        public Task<GameStatus> Play(short initialLevel, CancellationToken cancellationToken);
        public Task<bool> ShouldPlayAgain(CancellationToken cancellationToken);
        public Task Terminate(CancellationToken cancellationToken);
    } 
}