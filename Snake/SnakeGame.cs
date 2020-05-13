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
        private readonly IInputOutputService _inputOutputService;
        private readonly MainView _mainView;
        private readonly HelpView _helpView;
        private readonly SummaryView _summaryView;
        // private (int startX, int startY, int width, int height) _helpPane;
        private bool _initialized;

        // private int _screenHeight;

        public SnakeGame(IInputOutputService io, MainView mainView, HelpView helpView, SummaryView summaryView) 
        {
            _inputOutputService = io;
            _mainView = mainView;
            _helpView = helpView;
            _summaryView = summaryView;
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
            _helpView.SetDimensions(
                Constants.PADDING, 
                Constants.PADDING,
                Constants.HELP_PANE_WIDTH,
                Constants.MAIN_SCREEN_MIN_HEIGHT * Constants.MAIN_SCREEN_SCALE);

            // Top Right
            _mainView.SetDimensions(
                Constants.PADDING + Constants.HELP_PANE_WIDTH + Constants.PADDING, 
                Constants.PADDING,
                Constants.MAIN_SCREEN_MIN_WIDTH * Constants.MAIN_SCREEN_SCALE,
                Constants.MAIN_SCREEN_MIN_HEIGHT * Constants.MAIN_SCREEN_SCALE);

            // Bottom
            _summaryView.SetDimensions(
                Constants.PADDING,
                Constants.PADDING + Constants.MAIN_SCREEN_MIN_HEIGHT * Constants.MAIN_SCREEN_SCALE + Constants.PADDING,
                Constants.HELP_PANE_WIDTH + Constants.PADDING + Constants.MAIN_SCREEN_MIN_WIDTH * Constants.MAIN_SCREEN_SCALE,
                Constants.SUMMARY_PANE_HEIGHT);


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
            await _summaryView.DrawBorder(cancellationToken);
            await _helpView.DrawBorder(cancellationToken);

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