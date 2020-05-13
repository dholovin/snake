using System;
using System.Threading;
using System.Threading.Tasks;
using Snake.Common;
using Snake.Views;
using Snake.ServiceContracts.Interfaces;

namespace Snake
{
    internal class SnakeGame : BaseComponent, IGame
    {
        private IInputOutputService _inputOutputService;
        private MainView _mainView;
        private HelpView _helpView;
        private SummaryView _summaryView;
        // private (int startX, int startY, int width, int height) _helpPane;
        private bool _initialized;

        // private int _screenHeight;

        public SnakeGame(IInputOutputService io) 
        {
            _inputOutputService = io;
        }

        public async Task Initialize(CancellationToken cancellationToken)
        {
            // TODO: prompt for screen size -> adjust scale

            var minWidth = Constants.HELP_PANE_WIDTH + Constants.MAIN_SCREEN_MIN_WIDTH * Constants.MAIN_SCREEN_SCALE + Constants.PADDING * 2;
            var minHeight = Constants.MAIN_SCREEN_MIN_HEIGHT * Constants.MAIN_SCREEN_SCALE + Constants.PADDING * 2 + Constants.SUMMARY_PANE_HEIGHT;

            var (width, height) = await _inputOutputService.GetViewportDimensions(cancellationToken);
            if (width < minWidth || height < minHeight) 
            {
                throw new ArgumentException(String.Format(Constants.SCREEN_RESOLUTION_ERROR, minWidth, minHeight));
            }

            // Top Left
            _helpView = new HelpView(
                Constants.PADDING, 
                Constants.PADDING,
                Constants.HELP_PANE_WIDTH,
                Constants.MAIN_SCREEN_MIN_HEIGHT * Constants.MAIN_SCREEN_SCALE,
                _inputOutputService);

            // Top Right
            _mainView = new MainView(
                Constants.PADDING + Constants.HELP_PANE_WIDTH + Constants.PADDING, 
                Constants.PADDING,
                Constants.MAIN_SCREEN_MIN_WIDTH * Constants.MAIN_SCREEN_SCALE,
                Constants.MAIN_SCREEN_MIN_HEIGHT * Constants.MAIN_SCREEN_SCALE,
                _inputOutputService);

            // Bottom
            _summaryView = new SummaryView(
                0,
                Constants.PADDING + Constants.MAIN_SCREEN_MIN_HEIGHT * Constants.MAIN_SCREEN_SCALE + Constants.PADDING,
                Constants.MAIN_SCREEN_MIN_WIDTH * Constants.MAIN_SCREEN_SCALE,
                Constants.SUMMARY_PANE_HEIGHT,
                _inputOutputService);


            _initialized = true;
        }

        public async Task Play(CancellationToken cancellationToken)
        {
            if (!_initialized)
                await Initialize(cancellationToken);

            // await _inputOutputService.Clear(cancellationToken);
            // initBoardPanel()
            // initStatusPanel()
            // await boardPanel.DrawBoarder(cancellationToken);
            await _mainView.DrawBorder(cancellationToken);

            // await UpdateScoresAsync(scoresItem, cancellationToken);
            // bool? playAgain = null;
            // while (playAgain == null)
            // {
            //     await DrawAsync(cancellationToken);
            //     playAgain = await PlayerInputAsync(cancellationToken);
            // }

            // return await Task.FromResult((bool) playAgain);
        }

        public async Task<bool> ShouldPlayAgain(CancellationToken cancellationToken) 
        {
            return await _summaryView.ShouldPlayAgain(cancellationToken);
        }

        public async Task Terminate(CancellationToken cancellationToken)
        {
            await _inputOutputService.Terminate(cancellationToken);
        }
    }
}