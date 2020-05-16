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

        public short Level { get; set; }
        private int _score;
        public int Score { 
            get { return _score; }
            set {
                _score = value;
                _inputOutputService.Print(StartX + 1, StartY + 1, $"LEVEL {Level} : SCORE {value}", 
                    CancellationToken.None);
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

        public async Task Reset(short initialLevel, CancellationToken cancellationToken = default)
        {
            Level = initialLevel;
            Score = 0;
            await Task.CompletedTask;
        }
    } 
}