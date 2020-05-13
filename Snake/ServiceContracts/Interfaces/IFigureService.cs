using System.Threading;
using System.Threading.Tasks;

namespace Snake.ServiceContracts.Interfaces
{
    public interface IFigureService
    {
        public Task<string> GetTopHorizontBlock(CancellationToken cancellationToken);
        public Task<string> GetBottomHorizontBlock(CancellationToken cancellationToken);
        public Task<string> GetLeftVertBlock(CancellationToken cancellationToken);
        public Task<string> GetRightVertBlock(CancellationToken cancellationToken);
    } 
}