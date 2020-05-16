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
        private int _DEBUG_OnFoodHits = 0;
        private int _tickDelay => Constants.LEVEL_SPEED_MULTIPLIER * (10 - _summaryView.Level);

        public SnakeGame(IInputOutputService io, MainView mainView, HelpView helpView, SummaryView summaryView) 
        {
            _inputOutputService = io;
            _mainView = mainView;
            _helpView = helpView;
            _summaryView = summaryView;

            // TODO: Ensure we subscribe for events  only once
            _mainView.OnFoodHit += (sender, args) => {
                _summaryView.Score += Constants.SCORE_INCREMENT; 
                //  _DEBUG_OnFoodHits++;
                //  _inputOutputService.Print(0,0, _DEBUG_OnFoodHits ,CancellationToken.None);
            };
            _mainView.OnGameOver += (sender, args) => { _isGameActive = false; };
        }

        public async Task<(short, short)> AskForInitialSetup(CancellationToken cancellationToken = default)
        {
            await _inputOutputService.Clear(cancellationToken);
            await _inputOutputService.Print(Constants.PADDING, Constants.PADDING, Constants.INPUT_LEVEL, cancellationToken);
            
            short? playerLevel = null;
            while (playerLevel == null)
            {
                short result = -1;
                var input = await _inputOutputService.GetString(cancellationToken);
                if (short.TryParse(input, out result) && result >= 0 && result <= 9) 
                    playerLevel = result;
            }

            return await Task.FromResult(((short)playerLevel, (short)Constants.MAIN_SCREEN_SIZE_MULTIPLIER));
        }

        public async Task Initialize(short screenSizeMultiplier, CancellationToken cancellationToken = default)
        {
            var minWidth = Constants.HELP_PANE_WIDTH + Constants.MAIN_SCREEN_MIN_WIDTH * screenSizeMultiplier + Constants.PADDING * 2;
            var minHeight = Constants.MAIN_SCREEN_MIN_HEIGHT * screenSizeMultiplier + Constants.PADDING * 2 + Constants.SUMMARY_PANE_HEIGHT;

            // Check screen size compatibility 
            // var (width, height) = (80, 24);
            var (width, height) = await _inputOutputService.GetViewportDimensions(cancellationToken);
            if (width < minWidth || height < minHeight) 
                throw new ArgumentException(String.Format(Constants.SCREEN_RESOLUTION_ERROR, minWidth, minHeight));

            await _inputOutputService.Clear(cancellationToken);

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

            _initialized = true;
        }

        public async Task<GameStatus> Play(short initialLevel, CancellationToken cancellationToken = default)
        {
            if (!_initialized)
                throw new ArgumentException(Constants.NOT_INITIALIZED_ERROR);

            await _summaryView.Reset(initialLevel, cancellationToken);            
            await _mainView.Reset(cancellationToken);

            _isGameActive = true;

            // TODO: test Exception Handling for different approaches
            Task.Run(async () =>                        // .NET 4.5
            // ThreadPool.QueueUserWorkItem(async state => // .NET 1.1
            {
                await PlayerActionsHandler(cancellationToken);
            }, cancellationToken);

            while (_isGameActive && !cancellationToken.IsCancellationRequested)
            {
                await _mainView.Tick(cancellationToken);
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

                    if (playerAction == PlayerActionEnum.Terminate)
                    {
                        // Ctrl+C - terminate program, pass false
                        // _isGameActive = false; // TODO: check if THIS or Terminate() exits _mainView.Tick()
                        await Terminate(cancellationToken); 
                        break;
                    }
                    else if (playerAction == PlayerActionEnum.Quit)
                    {
                        // Ctrl+Z - quit program, pass true
                        // _isGameActive = false; // TODO: check if THIS or Terminate() exits _mainView.Tick()
                        await Terminate(cancellationToken); 
                        break;
                    }
                    else if (playerAction == PlayerActionEnum.ToggleHelpView)
                    {
                        // TODO: implement Toggle help view
                    }
                    else if (playerAction != PlayerActionEnum.None)
                    {
                        // Ignore PlayerActions geometrically opposite to current action
                        if (!(_mainView.CurrentAction == PlayerActionEnum.Left && playerAction == PlayerActionEnum.Right)
                            && !(_mainView.CurrentAction == PlayerActionEnum.Right && playerAction == PlayerActionEnum.Left)
                            && !(_mainView.CurrentAction == PlayerActionEnum.Up && playerAction == PlayerActionEnum.Down)
                            && !(_mainView.CurrentAction == PlayerActionEnum.Down && playerAction == PlayerActionEnum.Up)) {

                            // Update game action
                            _mainView.CurrentAction = playerAction; // TODO: fire some Event, maybe?
                            await _mainView.Tick(cancellationToken); // This would give a god speed if player holds a key :)
                        }
                    }
                    else 
                        continue;
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