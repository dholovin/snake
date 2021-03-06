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
        private readonly SummaryView _summaryView;
        private readonly HelpView _helpView;
        private bool _initialized;
        private bool _isGameActive;
        private int _tickDelay => Constants.SPEED_MULTIPLIER * (10 - _summaryView.Speed);
        private int _DEBUG_OnFoodHits = 0;

        public SnakeGame(IInputOutputService io, MainView mainView, SummaryView summaryView, HelpView helpView) 
        {
            _inputOutputService = io;
            _mainView = mainView;
            _summaryView = summaryView;
            _helpView = helpView;

            // Ensure we subscribe for events  only once so, inside constructor
            _mainView.OnFoodHit += (sender, args) => {
                var newScore = _summaryView.Score + Constants.SCORE_INCREMENT;
                if (newScore % Constants.SCORE_PER_SPEED_INCREMENT == 0 && _summaryView.Speed != 9)
                    _summaryView.Speed++;

                _summaryView.Score += Constants.SCORE_INCREMENT; 
                //  _DEBUG_OnFoodHits++;
                //  _inputOutputService.Print(0,0, _DEBUG_OnFoodHits ,CancellationToken.None);
            };
            _mainView.OnGameOver += (sender, args) => { _isGameActive = false; };
        }

        public async Task<short> AskForInitialSetup(CancellationToken cancellationToken = default)
        {
            await _inputOutputService.Clear(cancellationToken);
            await _inputOutputService.Print(Constants.PADDING, Constants.PADDING, Constants.INPUT_SPEED, cancellationToken);
            
            short? playerSpeed = null;
            while (playerSpeed == null)
            {
                short result = -1;
                var input = await _inputOutputService.GetString(cancellationToken);
                if (short.TryParse(input, out result) && result >= 0 && result <= 9) 
                    playerSpeed = result;
            }

            return await Task.FromResult(((short)playerSpeed));
        }

        public async Task Initialize(CancellationToken cancellationToken = default)
        {
            var minWidth = Constants.HELP_PANE_WIDTH + Constants.MAIN_SCREEN_MIN_WIDTH + Constants.PADDING * 2;
            var minHeight = Constants.MAIN_SCREEN_MIN_HEIGHT + Constants.PADDING * 2 + Constants.SUMMARY_PANE_HEIGHT;

            // Check screen size compatibility 
            var (width, height) = await _inputOutputService.GetViewportDimensions(cancellationToken);
            if (width < minWidth || height < minHeight) 
                throw new ArgumentException(String.Format(Constants.SCREEN_RESOLUTION_ERROR, minWidth, minHeight));

            await _inputOutputService.Clear(cancellationToken);

            // Initialize Views and set Dimensions 
            Task.WaitAll(new Task[3] {
                // Top Left View
                _summaryView.SetDimensions(
                    Constants.PADDING, 
                    Constants.PADDING,
                    Constants.HELP_PANE_WIDTH,
                    Constants.MAIN_SCREEN_MIN_HEIGHT, cancellationToken),
                // Top Right View
                _mainView.SetDimensions(
                    Constants.PADDING + Constants.HELP_PANE_WIDTH + Constants.PADDING, 
                    Constants.PADDING,
                    Constants.MAIN_SCREEN_MIN_WIDTH,
                    Constants.MAIN_SCREEN_MIN_HEIGHT, cancellationToken),
                // Bottom View
                _helpView.SetDimensions(
                    Constants.PADDING,
                    Constants.PADDING + Constants.MAIN_SCREEN_MIN_HEIGHT + Constants.PADDING,
                    Constants.HELP_PANE_WIDTH + Constants.PADDING + Constants.MAIN_SCREEN_MIN_WIDTH,
                    Constants.SUMMARY_PANE_HEIGHT, cancellationToken)
            });
            Task.WaitAll(new Task[3] {
                _summaryView.DrawBorder(cancellationToken),
                _mainView.DrawBorder(cancellationToken),
                _helpView.DrawBorder(cancellationToken)
            });

            _initialized = true;
        }

        public async Task<GameStatus> Play(short initialSpeed, CancellationToken cancellationToken = default)
        {
            if (!_initialized)
                throw new ArgumentException(Constants.NOT_INITIALIZED_ERROR);

            await _summaryView.Reset(initialSpeed, cancellationToken);            
            await _mainView.Reset(cancellationToken);

            _isGameActive = true;

            Task.Run(async () => 
            {
                await PlayerActionsHandler(cancellationToken);
                // NOTE: next line may help to mitigate concurrency issues, if any.
                //await Task.Delay(_tickDelay, cancellationToken); 
            }, cancellationToken);

            while (_isGameActive && !cancellationToken.IsCancellationRequested)
            {
                await _mainView.Tick(cancellationToken);
                await Task.Delay(_tickDelay, cancellationToken);
            }

            return await Task.FromResult(await GetGameStatus(_isGameActive, _summaryView, cancellationToken));
        }

        private async Task PlayerActionsHandler(CancellationToken cancellationToken = default)
        {
                while (_isGameActive && !cancellationToken.IsCancellationRequested)
                {
                    var playerAction = await _inputOutputService.GetPlayerAction(cancellationToken);

                    if (playerAction == PlayerActionEnum.Terminate)
                    {
                        // Ctrl+C - terminate program, pass false
                        await Terminate(cancellationToken); 
                        break;
                    }
                    else if (playerAction == PlayerActionEnum.Quit)
                    {
                        // Ctrl+Z - quit program, pass true
                        await Terminate(cancellationToken); 
                        break;
                    }
                    else if (playerAction == PlayerActionEnum.SpeedUp)
                    {
                        if( _summaryView.Speed != 9) 
                        {
                            _summaryView.Speed++;
                            await _summaryView.ShowScore(cancellationToken);
                        }
                    } 
                    else if (playerAction != PlayerActionEnum.None)
                    {
                        // Ignore PlayerActions geometrically opposite to current action
                        if (!(_mainView.CurrentAction == PlayerActionEnum.Left && playerAction == PlayerActionEnum.Right)
                            && !(_mainView.CurrentAction == PlayerActionEnum.Right && playerAction == PlayerActionEnum.Left)
                            && !(_mainView.CurrentAction == PlayerActionEnum.Up && playerAction == PlayerActionEnum.Down)
                            && !(_mainView.CurrentAction == PlayerActionEnum.Down && playerAction == PlayerActionEnum.Up)) {

                            _mainView.CurrentAction = playerAction; // TODO: fire some Event, maybe?

                            // NOTE: Below code would result in 'god speed' if player holds a key BUT may cause concurrency issues
                            await _mainView.Tick(cancellationToken); 
                        }
                    }
                    else 
                        continue;
                }

            await Task.CompletedTask;
        }

        public async Task<bool> ShouldPlayAgain(CancellationToken cancellationToken = default) 
        {
            await _inputOutputService.Print(_helpView.StartX + 1, _helpView.StartY + _helpView.Height - 1, 
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
                        Speed = summaryView.Speed,
                        Score = summaryView.Score
                    });
            else {
                return await Task.FromResult(new GameStatus()
                    {
                        IsActive = false,
                        Speed = 0,
                        Score = 0
                    });
            }
        }
    }
}