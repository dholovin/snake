using System.Threading;
using System.Threading.Tasks;
using Snake.Common;
using Snake.ServiceContracts.Interfaces;

namespace Snake.ServiceContracts
{
    public class FigureService : BaseComponent, IFigureService
    {
        public async Task<string> GetTopHorizontBlock(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult("▄");
        }

        public async Task<string> GetBottomHorizontBlock(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult("▀");
        }

        public async Task<string> GetLeftVertBlock(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult("▌");
        }

        public async Task<string> GetRightVertBlock(CancellationToken cancellationToken)
        {
            return await Task.FromResult("▐");
        }
    }
}