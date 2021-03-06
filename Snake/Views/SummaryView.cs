using System.Threading;
using System.Threading.Tasks;
using Snake.Common;
using Snake.ServiceContracts.Interfaces;

namespace Snake.Views
{
    public class SummaryView : BaseView
    {
        private readonly IInputOutputService _inputOutputService;
        private readonly IFigureService _figureService;

        public short Speed { get; set; }
        private int _score;
        public int Score { 
            get { return _score; }
            set {
                _score = value;
                ShowScore().Wait();
            }
        }

        public SummaryView(IInputOutputService inputOutputService, IFigureService figureService)
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
        }

        public async Task Reset(short initialSpeed, CancellationToken cancellationToken = default)
        {
            Speed = initialSpeed;
            Score = 0;
            await Task.CompletedTask;
        }
        
        public async Task ShowScore(CancellationToken cancellationToken = default)
        {
            Task.WaitAll(new Task[2] {
                _inputOutputService.Print(StartX + 1, StartY + 1, $"SCORE : {Score}", cancellationToken),
                _inputOutputService.Print(StartX + 1, StartY + 2, $"SPEED : {Speed}", cancellationToken)
            });
            await Task.CompletedTask;
        }
    } 
}