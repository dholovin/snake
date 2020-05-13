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

        public SummaryView(IInputOutputService inputOutputService, IFigureService figureService)
        {
            _inputOutputService = inputOutputService;
            _figureService = figureService;
        }

        /* Key Codes:
            n/N     110/78
            y/Y     121/89
            CTRL+C  3
            ESC     27
            ENTER   13
        */
        public async Task<bool> ShouldPlayAgain(CancellationToken cancellationToken)
        {
            byte? key = null;
            await _inputOutputService.Print(StartX + 1, StartY + Height - 1, Constants.SHOULD_PLAY_AGAIN, cancellationToken);

            // TODO: should Key Codes be a part of IInputOutputService, though, not every implementation might have KeyCodes?
            while (key != 27 && key != 110 && key != 78 && key != 121 && key != 89 && key != 3) // 'ESC'|n|N|y|Y|CTRL+C
            {
                if (key != null) {
                    // await _inputOutputService.Print(String.Format("Unsupported key '{0}'", key.ToString()), cancellationToken);
                }

                key = await _inputOutputService.GetKey(cancellationToken);
            }

            return await Task.FromResult(key == 89 || key == 121); // 'true' if 'Y'/'y'
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