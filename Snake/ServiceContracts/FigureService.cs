using System.Threading;
using System.Threading.Tasks;
using Snake.Common;
using Snake.ServiceContracts.Interfaces;

namespace Snake.ServiceContracts
{
    public class FigureService : BaseService, IFigureService
    {
        public async Task<string> GetTopHorizontFigure(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult("▄");
            // return await Task.FromResult("▀");
        }

        public async Task<string> GetBottomHorizontFigure(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult("▀");
            //return await Task.FromResult("▄");
        }

        public async Task<string> GetLeftVertFigure(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult("▌");
        }

        public async Task<string> GetRightVertFigure(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult("▐");
        }

        public async Task<string> GetSnakeFigure(CancellationToken cancellationToken = default)
        {
            //return await Task.FromResult("§");
            return await Task.FromResult("░");
            //return await Task.FromResult("▓");
            
        }
        public async Task<string> GetSnakeHeadFigure(CancellationToken cancellationToken = default)
        {
            //return await Task.FromResult("☻");
            return await Task.FromResult("█");
        }

        public async Task<string> GetFoodFigure(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult("$");
            // return await Task.FromResult("*");
        }

        public async Task<string> GetDeadFoodFigure(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult("†");
            // return await Task.FromResult("╬");
        }
    }
}