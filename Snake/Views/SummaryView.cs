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

        private int _score;
        public int Score { 
            get { return _score; }
            set {
                _score = value;
                _inputOutputService.Print(StartX + 1, StartY + Height - 2, "SCORE: " + value, CancellationToken.None);
            }
        }
        public short Level { get; set; }

        public SummaryView(IInputOutputService inputOutputService, IFigureService figureService)
        {
            _inputOutputService = inputOutputService;
            _figureService = figureService;
            Score = 0; // default
            Level = 0; // default
        }

        public async Task DrawBorder(CancellationToken cancellationToken) {
            string topHorizontBlock = await _figureService.GetTopHorizontFigure(cancellationToken);          // "▄";
            string bottomHorizontBlock = await _figureService.GetBottomHorizontFigure(cancellationToken);    // "▀";
            string leftVertBlock = await _figureService.GetLeftVertFigure(cancellationToken);                // "▌";
            string rightVertBlock = await _figureService.GetRightVertFigure(cancellationToken);              // "▐";

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

            await _inputOutputService.Print(StartX + 1, StartY + 1, "SUMMARY VIEW",  cancellationToken);
        }
    } 
}