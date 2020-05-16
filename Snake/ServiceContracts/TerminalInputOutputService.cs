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

        public async Task Print(int x, int y, object message, CancellationToken cancellationToken = default)
        {
            MoveCursorTo(x, y);
            Out(message.ToString());

            await Task.CompletedTask;
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
            else if (key == 32)                                             // SPACE - toggle help view
                return await Task.FromResult(PlayerActionEnum.ToggleHelpView);
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

            // else if (key == 48) // 0 - show/hide help screen
            //     await _helpBoard.SetVisibleAsync(!_helpBoard.Visible, cancellationToken);
            // else if (key == 49) // 1 - show/hide next figure
            //     await _glass.ShowHideNextAsync(cancellationToken);
            // else if (key == 52) // 4 - next level
            //     await _scoreBoard.NextLevelAsync(cancellationToken);
            // else if (key == 55 || key == 68) // 7 - left 
            //     playerAction = PlayerActionEnum.Left;
            // else if (key == 57 || key == 67) // 9 - right 
            //     playerAction = PlayerActionEnum.Right;
            // else if (key == 56 || key == 65) // 8 - rotate 
            //     playerAction = PlayerActionEnum.Rotate;
            // else if (key == 53 || key == 66) // 5 - soft drop 
            //     playerAction = PlayerActionEnum.SoftDrop;
            // else if (key == 32) // SPACE - drop
            //     playerAction = PlayerActionEnum.Drop;
        }
    }
}