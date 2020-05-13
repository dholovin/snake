using System.Threading;
using System.Threading.Tasks;
using Snake.Common;
using Snake.ServiceContracts.Interfaces;

namespace Snake.Views
{
    public class MainView : BasePanel
    {
        private IInputOutputService _inputOutputService;

        public MainView(int startX, int startY, int width, int height, IInputOutputService io) 
            : base(startX, startY, width, height)
        {
            _inputOutputService = io;
        }

        public async Task DrawBorder(CancellationToken cancellationToken) {
            // ֍ ۞ † ☼ □
            // string dashBlock = "▬";
            const string topHorizontBlock = "▄";
            const string bottomHorizontBlock = "▀";
            const string leftVertBlock = "▌";
            const string rightVertBlock = "▐";

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

            // if (init)
            // {
            //     int i;
            //     for (i = DeltaY; i < _glassArray.GetUpperBound(1) + 1 + DeltaY; i++)
            //         await IO.OutAsync(DeltaX - 2, i, Strings.GlassItem, cancellationToken);
            //     await IO.OutAsync(DeltaX - 2, i, Strings.GlassBottom1, cancellationToken);
            //     await IO.OutAsync(DeltaX - 2, i + 1, Strings.GlassBottom2, cancellationToken);
            // }
            // else
            // {
            //     for (var y = 0; y <= _glassArray.GetUpperBound(1); y++)
            //     {
            //         string line = null;
            //         for (var x = 0; x <= _glassArray.GetUpperBound(0); x++)
            //             if (_glassArray[x, y] == 0)
            //                 line += Strings.EmptyBox;
            //             else
            //                 line += Strings.BlockBox;
            //         await IO.OutAsync(DeltaX, y + DeltaY, line, cancellationToken);
            //     }
            // }
        }
    } 
}