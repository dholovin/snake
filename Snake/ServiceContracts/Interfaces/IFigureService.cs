using System.Threading;
using System.Threading.Tasks;

namespace Snake.ServiceContracts.Interfaces
{
    public interface IFigureService
    {
        public Task<string> GetTopHorizontFigure(CancellationToken cancellationToken);
        public Task<string> GetBottomHorizontFigure(CancellationToken cancellationToken);
        public Task<string> GetLeftVertFigure(CancellationToken cancellationToken);
        public Task<string> GetRightVertFigure(CancellationToken cancellationToken);
        public Task<string> GetSnakeFigure(CancellationToken cancellationToken);
        public Task<string> GetSnakeHeadFigure(CancellationToken cancellationToken);
        public Task<string> GetFoodFigure(CancellationToken cancellationToken);
        public Task<string> GetDeadFoodFigure(CancellationToken cancellationToken);
    } 
}