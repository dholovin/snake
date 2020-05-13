using System;
using System.Threading;
using System.Threading.Tasks;
using Snake.Common;
using Snake.ServiceContracts.Interfaces;
using static System.Terminal;

namespace Snake.ServiceContracts
{
    public class TerminalInputOutputService : BaseComponent, IInputOutputService
    {
        public async Task Clear(CancellationToken cancellationToken = default)
        {
            ClearScreen();

            await Task.CompletedTask;
        }

        public async Task<byte?> GetKey(CancellationToken cancellationToken = default)
        {
            if (!IsRawMode)
            {
                SetRawMode(true, true);

                // IsCursorVisible = false;
                IsCursorVisible = true;
                IsCursorBlinking = false;
            }

            var key = ReadRaw();

            return await Task.FromResult(key);
        }

        public async Task<string> GetString(CancellationToken cancellationToken = default)
        {
            // TODO: analyze IsRawMode usage
            // if (IsRawMode)
            // {
            //     SetRawMode(false, true);

            //     IsCursorVisible = true;
            //     IsCursorBlinking = true;
            // }

            var result = ReadLine();

            return await Task.FromResult(result);
        }

        public async Task<(int Width, int Height)> GetViewportDimensions(CancellationToken cancellationToken = default)
        {
            var output = (Size.Width, Size.Height);

            return await Task.FromResult(output);
        }

        public async Task Print(string message, CancellationToken cancellationToken = default)
        {
            Out(message);

            await Task.CompletedTask;
        }

        public async Task PrintAtNextLine(string message, CancellationToken cancellationToken = default)
        {
            OutLine(message);

            await Task.CompletedTask;
        }

        public async Task PrintAtNextLine(int x, int y, string message, CancellationToken cancellationToken = default)
        {
            MoveCursorTo(x, y);
            OutLine(message);

            await Task.CompletedTask;
        }

        public async Task Print(int x, int y, string message, CancellationToken cancellationToken = default)
        {
            MoveCursorTo(x, y);
            Out(message);

            await Task.CompletedTask;
        }

        public async Task Terminate(CancellationToken cancellationToken = default)
        {
            GenerateBreakSignal(TerminalBreakSignal.Interrupt);
            // GenerateBreakSignal(TerminalBreakSignal.Quit);
            await Task.CompletedTask;
        }
    }
}