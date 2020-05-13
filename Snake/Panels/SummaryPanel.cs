using System;
using System.Threading;
using System.Threading.Tasks;
using Snake.Common;
using Snake.ServiceContracts.Interfaces;

namespace Snake.Panels
{
    public class SummaryPanel : BasePanel
    {
        private IInputOutputService _inputOutputService;

        public SummaryPanel(int startX, int startY, int width, int height, IInputOutputService io) 
            : base(startX, startY, width, height)
        {
            _inputOutputService = io;
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
            
            // TODO: it should always know where to draw based on detail panel location
            await _inputOutputService.PrintAtNextLine(StartX, StartY, 
                "Press 'Y' to play again. Press 'N' or ESC to exit", cancellationToken);

            while (key != 27 && key != 110 && key != 78 && key != 121 && key != 89 && key != 3) // 'ESC'|n|N|y|Y|CTRL+C
            {
                if (key != null)
                    await _inputOutputService.PrintAtNextLine(String.Format("Unsupported key '{0}'", key.ToString()), cancellationToken);

                key = await _inputOutputService.GetKey(cancellationToken);
            }

            return await Task.FromResult(key == 89 || key == 121); // 'true' if 'Y'/'y'
        }
    } 
}