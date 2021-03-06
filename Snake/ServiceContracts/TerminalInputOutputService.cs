using System;
using System.Threading;
using System.Threading.Tasks;
using Snake.Common;
using Snake.Common.Enums;
using Snake.ServiceContracts.Interfaces;
using static System.Terminal;

namespace Snake.ServiceContracts
{
    public class TerminalInputOutputService : BaseService, IInputOutputService
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

                IsCursorVisible = false;
                IsCursorBlinking = false;
            }

            var key = ReadRaw();

            return await Task.FromResult(key);
        }

        public async Task<string> GetString(CancellationToken cancellationToken = default)
        {
            if (IsRawMode)
            {
                SetRawMode(false, false);
                IsCursorVisible = true;
                IsCursorBlinking = true;
            }

            var result = ReadLine();

            return await Task.FromResult(result);
        }

        public async Task<(int Width, int Height)> GetViewportDimensions(CancellationToken cancellationToken = default)
        {
            var output = (Size.Width, Size.Height);

            return await Task.FromResult(output);
        }

        public async Task Print(object message, CancellationToken cancellationToken = default)
        {
            Out(message.ToString());

            await Task.CompletedTask;
        }

        public async Task Print(int x, int y, object message, CancellationToken cancellationToken = default)
        {
            MoveCursorTo(x, y);
            Out(message.ToString());

            await Task.CompletedTask;
        }

        public async Task Print(int x, int y, (byte R, byte G, byte B) color, object message, 
            CancellationToken cancellationToken = default)
        {
            ForegroundColor(color.R, color.G, color.B);
            await Print(x, y, message, cancellationToken);
            ForegroundColor(255, 255, 255);
        }

        public async Task PrintAtNextLine(object message, CancellationToken cancellationToken = default)
        {
            OutLine(message.ToString());

            await Task.CompletedTask;
        }

        public async Task PrintAtNextLine(int x, int y, object message, CancellationToken cancellationToken = default)
        {
            MoveCursorTo(x, y);
            OutLine(message.ToString());

            await Task.CompletedTask;
        }

        public async Task PrintAtNextLine(int x, int y, (byte R, byte G, byte B) color, object message,
            CancellationToken cancellationToken)
        {
            ForegroundColor(color.R, color.G, color.B);
            await PrintAtNextLine(message, cancellationToken);
            ForegroundColor(255, 255, 255);
        }

        public async Task Terminate(CancellationToken cancellationToken = default)
        {
            GenerateBreakSignal(TerminalBreakSignal.Interrupt);
            // GenerateBreakSignal(TerminalBreakSignal.Quit);
            await Task.CompletedTask;
        }

        public async Task<PlayerActionEnum> GetPlayerAction(CancellationToken cancellationToken = default)
        {
            var key = await GetKey(cancellationToken);

            // TODO: if ever need to log/display pressed key code
            // if (key != null)
            //     await Print(String.Format(" Key Pressed '{0}'", key.ToString()), cancellationToken);

            if (key == 3 || key == 110 || key == 78)                        // Ctrl+C, 'n', 'N' - terminate program
                return await Task.FromResult(PlayerActionEnum.Terminate);
            else if (key == 0)                                              // Ctrl+Z - quit program
                return await Task.FromResult(PlayerActionEnum.Quit);
            else if (key == 121 || key == 89)                               // 'y', 'Y' - replay
                return await Task.FromResult(PlayerActionEnum.PlayAgain);
            else if (key == 32)                                             // SPACE - Speed Up
                return await Task.FromResult(PlayerActionEnum.SpeedUp);
            else if (key == 68 || key == 52)                                // LeftArrow or '4'
                return await Task.FromResult(PlayerActionEnum.Left);
            else if (key == 67 || key == 54)                                // RightArrow or '6'
                return await Task.FromResult(PlayerActionEnum.Right);
            else if (key == 66 || key == 50)                                // DownArrow or '2'
                return await Task.FromResult(PlayerActionEnum.Down);
            else if (key == 65 || key == 56)                                // UpArrow or '8'
                return await Task.FromResult(PlayerActionEnum.Up);
            else
                return await Task.FromResult(PlayerActionEnum.None);            
        }
    }
}