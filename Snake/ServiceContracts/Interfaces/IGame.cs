using System.Threading;
using System.Threading.Tasks;

namespace Snake.ServiceContracts.Interfaces
{
    public interface IGame
    {
        public Task Initialize(CancellationToken cancellationToken);
        public Task Play(CancellationToken cancellationToken);
        public Task<bool> ShouldPlayAgain(CancellationToken cancellationToken);
        public Task Terminate(CancellationToken cancellationToken);   
    } 
}