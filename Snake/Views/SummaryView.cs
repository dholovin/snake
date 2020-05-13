using System;
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

        public int Score { get; private set; }
        public short Level { get; private set; }

        public SummaryView(IInputOutputService inputOutputService, IFigureService figureService)
        {
            _inputOutputService = inputOutputService;
            _figureService = figureService;
            Score = 0; // default
            Level = 0; // default
        }

        public async Task UpdateSummary(int score, short level, CancellationToken cancellationToken) 
        {
            Score = score;
            Level = level;

            await Task.CompletedTask;
        }

        public async Task SetScore(int score, CancellationToken cancellationToken) 
        {
            Score = score;
            await Task.CompletedTask;
        }
        public async Task SetLevel(short level, CancellationToken cancellationToken) 
        {
            Level = level;
            await Task.CompletedTask;
        }

        public async Task DrawBorder(CancellationToken cancellationToken) {
            string topHorizontBlock = await _figureService.GetTopHorizontBlock(cancellationToken);          // "▄";
            string bottomHorizontBlock = await _figureService.GetBottomHorizontBlock(cancellationToken);    // "▀";
            string leftVertBlock = await _figureService.GetLeftVertBlock(cancellationToken);                // "▌";
            string rightVertBlock = await _figureService.GetRightVertBlock(cancellationToken);              // "▐";

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