using System;
using System.Threading;
using System.Threading.Tasks;
using Snake.Common;
using Snake.Common.Enums;
using Snake.Views;
using Snake.ServiceContracts.Interfaces;
using Snake.Models;

namespace Snake
{
    internal class SnakeGame : IGame
    {
        private readonly IInputOutputService _inputOutputService;
        private readonly MainView _mainView;
        private readonly HelpView _helpView;
        private readonly SummaryView _summaryView;

        private bool _initialized;
        private bool _isGameActive;
        private int _tickDelay => Constants.LEVEL_SPEED_MULTIPLIER * (10 - _summaryView.Level);

        // private int _screenHeight;

        public SnakeGame(IInputOutputService io, MainView mainView, HelpView helpView, SummaryView summaryView) 
        {
            _inputOutputService = io;
            _mainView = mainView;
            _helpView = helpView;
            _summaryView = summaryView;
        }

        public async Task<(short, short)> AskForInitialSetup(CancellationToken cancellationToken = default)
        {
            // TODO: implement
            return await Task.FromResult(((short)0, (short)Constants.MAIN_SCREEN_SIZE_MULTIPLIER));
        }

        private async Task Initialize(short initialLevel, short screenSizeMultiplier, 
            CancellationToken cancellationToken = default)
        {
            var minWidth = Constants.HELP_PANE_WIDTH + Constants.MAIN_SCREEN_MIN_WIDTH * screenSizeMultiplier + Constants.PADDING * 2;
            var minHeight = Constants.MAIN_SCREEN_MIN_HEIGHT * screenSizeMultiplier + Constants.PADDING * 2 + Constants.SUMMARY_PANE_HEIGHT;

            // Check screen size compatibility 
            var (width, height) = await _inputOutputService.GetViewportDimensions(cancellationToken);
            if (width < minWidth || height < minHeight) 
                throw new ArgumentException(String.Format(Constants.SCREEN_RESOLUTION_ERROR, minWidth, minHeight));

            
            await _summaryView.SetLevel(initialLevel, cancellationToken);

            // Initialize Views and set dimensions 
            Task.WaitAll(new Task[3] {
                // Top Left View
                _helpView.SetDimensions(
                    Constants.PADDING, 
                    Constants.PADDING,
                    Constants.HELP_PANE_WIDTH,
                    Constants.MAIN_SCREEN_MIN_HEIGHT * screenSizeMultiplier, cancellationToken),
                // Top Right View
                _mainView.SetDimensions(
                    Constants.PADDING + Constants.HELP_PANE_WIDTH + Constants.PADDING, 
                    Constants.PADDING,
                    Constants.MAIN_SCREEN_MIN_WIDTH * screenSizeMultiplier,
                    Constants.MAIN_SCREEN_MIN_HEIGHT * screenSizeMultiplier, cancellationToken),
                // Bottom View
                _summaryView.SetDimensions(
                    Constants.PADDING,
                    Constants.PADDING + Constants.MAIN_SCREEN_MIN_HEIGHT * screenSizeMultiplier + Constants.PADDING,
                    Constants.HELP_PANE_WIDTH + Constants.PADDING + Constants.MAIN_SCREEN_MIN_WIDTH * screenSizeMultiplier,
                    Constants.SUMMARY_PANE_HEIGHT, cancellationToken)
            });
            Task.WaitAll(new Task[3] {
                _helpView.DrawBorder(cancellationToken),
                _mainView.DrawBorder(cancellationToken),
                _summaryView.DrawBorder(cancellationToken)
            });
            
            

            // Subscribe to Events
            _mainView.OnGameFinished += (sender, args) => { _isGameActive = false; };
            //  _glass.OnFullLine += async (sender, args) =>
            // {
            //     _scoreBoard.Lines++;
            //     _levelSwitch++;
            //     if (_levelSwitch != Constants.LinesNextLevelSwitch) return;
            //     await _scoreBoard.NextLevelAsync(cancellationToken);
            // };
            // _glass.OnNewBlock += (sender, block) => { _scoreBoard.Score += 10; };

            _initialized = true;
        }

        public async Task<GameStatus> Play(short initialLevel, short screenSizeMultiplier, 
            CancellationToken cancellationToken = default)
        {
            if (!_initialized)
                // throw new ArgumentException(Constants.NOT_INITIALIZED_ERROR);
                await Initialize(initialLevel, screenSizeMultiplier, cancellationToken);

            // await IO.ClearAsync(cancellationToken);
            // await _helpBoard.ResetAsync(cancellationToken);
            // await _scoreBoard.ResetAsync(playerLevel, cancellationToken);
            // await _glass.ResetAsync(cancellationToken);

            _isGameActive = true;

            
            // TODO: check difference between Exception Handling
            Task.Run(async () =>                        // .NET 4.5
            // ThreadPool.QueueUserWorkItem(async state => // .NET 1.1
            {
                await PlayerActionsHandler(cancellationToken);
            }, cancellationToken);

            while (_isGameActive && !cancellationToken.IsCancellationRequested)
            {
                // Draw board and start moving Snake
                await _mainView.Tick(PlayerActionEnum.None, cancellationToken);
                await Task.Delay(_tickDelay, cancellationToken);
            }

            // var playerName = await ReadPlayerNameAsync(cancellationToken);
            // var result = await _scoreBoard.ToLeaderBoardItemAsync(playerName, cancellationToken);
            return await Task.FromResult(await GetGameStatus(_isGameActive, _summaryView, cancellationToken));
        }

        private async Task PlayerActionsHandler(CancellationToken cancellationToken = default)
        {
            // // TODO: check difference between Exception Handling
            // Task.Run(async () =>                        // .NET 4.5
            // // ThreadPool.QueueUserWorkItem(async state => // .NET 1.1
            // {
                while (_isGameActive && !cancellationToken.IsCancellationRequested)
                {
                    var playerAction = await _inputOutputService.GetPlayerAction(cancellationToken);

                    // if (playerAction == PlayerActionEnum.Terminate)
                    // {
                    //     await Terminate(cancellationToken); // Ctrl+C - terminate program, pass false
                    //     break;
                    // }
                    // else if (playerAction == PlayerActionEnum.Quit)
                    // {
                    //     await Terminate(cancellationToken); // Ctrl+Z - quit program, pass true
                    //     break;
                    // }
                    // else if (playerAction == PlayerActionEnum.ToggleHelpView) //  toggle help view
                    // {
                    //     // TODO: implement
                    // }    
                    // else if (playerAction != PlayerActionEnum.None) //  toggle help view
                    //     await _mainView.Tick(playerAction, cancellationToken);
                    // else 
                    //     continue;
                }
            // }, cancellationToken);

            await Task.CompletedTask;
        }

        public async Task<bool> ShouldPlayAgain(CancellationToken cancellationToken = default) 
        {
            await _inputOutputService.Print(_summaryView.StartX + 1, _summaryView.StartY + _summaryView.Height - 1, 
                Constants.SHOULD_PLAY_AGAIN, cancellationToken);

            var playerAction = PlayerActionEnum.None;

            while (playerAction != PlayerActionEnum.Quit && playerAction != PlayerActionEnum.Terminate
                && playerAction != PlayerActionEnum.PlayAgain)
            {
                playerAction = await _inputOutputService.GetPlayerAction(cancellationToken);
            }

            return await Task.FromResult(playerAction == PlayerActionEnum.PlayAgain);
        }

        public async Task Terminate(CancellationToken cancellationToken = default)
        {
            await _inputOutputService.Terminate(cancellationToken);
        }

        public async Task<GameStatus> GetGameStatus(bool isActive, SummaryView summaryView, 
            CancellationToken cancellationToken = default) 
        { 
            if (_initialized) 
                return await Task.FromResult(new GameStatus()
                    {
                        IsActive = isActive,
                        Level = _summaryView.Level,
                        Score = _summaryView.Score
                    });
            else {
                return await Task.FromResult(new GameStatus()
                    {
                        IsActive = false,
                        Level = 0,
                        Score = 0
                    });
            }
        }
    }
}