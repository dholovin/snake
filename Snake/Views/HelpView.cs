using System.Threading;
using System.Threading.Tasks;
using Snake.Common;
using Snake.ServiceContracts.Interfaces;

namespace Snake.Views
{
    public class HelpView : BaseView
    {
        private readonly IInputOutputService _inputOutputService;
        private readonly IFigureService _figureService;

        public HelpView(IInputOutputService inputOutputService, IFigureService figureService)
        {
            _inputOutputService = inputOutputService;
            _figureService = figureService;
        }

        public async Task DrawBorder(CancellationToken cancellationToken) {
            string topHorizontBlock = await _figureService.GetTopHorizontFigure(cancellationToken);
            string bottomHorizontBlock = await _figureService.GetBottomHorizontFigure(cancellationToken);
            string leftVertBlock = await _figureService.GetLeftVertFigure(cancellationToken);
            string rightVertBlock = await _figureService.GetRightVertFigure(cancellationToken);

            for (int y = StartY; y < StartY + Height; y++)
            {
                // Left
                await _inputOutputService.Print(StartX, y, leftVertBlock,  cancellationToken);
                // Right
                await _inputOutputService.Print(StartX + Width, y, rightVertBlock,  cancellationToken);
            }
            
            for (int x = StartX; x <= StartX + Width /*+ 1*/; x++)
            {
                // Top
                await _inputOutputService.Print(x, StartY, topHorizontBlock,  cancellationToken);
                // Bottom
                await _inputOutputService.Print(x, StartY + Height, bottomHorizontBlock,  cancellationToken);
            }

            Task.WaitAll(new Task[3] {
                _inputOutputService.Print(StartX + 1, StartY + 1, "4: LEFT  6: RIGHT  8: UP  2: DOWN", cancellationToken),
                _inputOutputService.Print(StartX + 1, StartY + 2, "CTRL + Z or CTRL + C: QUIT", cancellationToken),
                _inputOutputService.Print(StartX + 1, StartY + 3, "SPACE: SPEEDUP", cancellationToken)
            });
            await Task.CompletedTask;
        }
    } 
}