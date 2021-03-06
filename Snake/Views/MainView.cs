using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Snake.Common;
using Snake.Common.Enums;
using Snake.ServiceContracts.Interfaces;

namespace Snake.Views
{
    public class MainView : BaseView
    {
        private readonly IInputOutputService _inputOutputService;
        private readonly IFigureService _figureService;

        private readonly string _snakePiece;
        private readonly string _snakeHeadPiece;
        private readonly string _foodPiece;
        private readonly string _deadFoodPiece;
        private readonly Random _randomizer;

        // TODO: challenge yourself and redesigning to use arrays ;)
        // private (int X, int Y)[] _snake; // ushort 0 to 65,535, 16 bit
        private List<(int X, int Y)> _snake = new List<(int X, int Y)>(); // TODO: Ensure thread-safety
        private List<(int X, int Y)> _food = new List<(int X, int Y)>(); // TODO: Ensure thread-safety
        private List<(int X, int Y)> _deadFood = new List<(int X, int Y)>(); // TODO: Ensure thread-safety
        
        public PlayerActionEnum CurrentAction { get; set; }
        public event EventHandler OnGameOver;
        public event EventHandler OnFoodHit;

        public MainView(IInputOutputService inputOutputService, IFigureService figureService)
        {
            _inputOutputService = inputOutputService;
            _figureService = figureService;
            
            _randomizer = new Random();
            _snakePiece = _figureService.GetSnakeFigure(CancellationToken.None).Result;
            _snakeHeadPiece = _figureService.GetSnakeHeadFigure(CancellationToken.None).Result;
            _foodPiece = _figureService.GetFoodFigure(CancellationToken.None).Result;
            _deadFoodPiece = _figureService.GetDeadFoodFigure(CancellationToken.None).Result;
        }

        public async Task DrawBorder(CancellationToken cancellationToken = default) {
            string topHorizontBlock = await _figureService.GetTopHorizontFigure(cancellationToken);
            string bottomHorizontBlock = await _figureService.GetBottomHorizontFigure(cancellationToken);
            string leftVertBlock = await _figureService.GetLeftVertFigure(cancellationToken);
            string rightVertBlock = await _figureService.GetRightVertFigure(cancellationToken);
            

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
        }

        override public async Task SetDimensions(int startX, int startY, int width, int height,
            CancellationToken cancellationToken = default)
        {
            await base.SetDimensions(startX, startY, width, height, cancellationToken);
            //await _inputOutputService.Print(0, 0, new Random().Next(100), cancellationToken); // as

            // Init Snake position
            var sn_x = startX + width/2;
            var sn_y = startY + height/2;
            // _snake = new[] {  (sn_x, sn_y), (sn_x - 1, sn_y), (sn_x - 2, sn_y) };
            _snake = new List<(int X, int Y)>()
            {
                (sn_x - 3, sn_y),(sn_x - 2, sn_y), (sn_x - 1, sn_y), (sn_x, sn_y)
            };

            // Draw Initial Snake
            Task.WaitAll(
                _snake.Select(coord => _inputOutputService.Print(coord.X, coord.Y, _snakePiece, cancellationToken)).ToArray()
            );
        }

        public async Task Reset(CancellationToken cancellationToken = default)
        {
            _food.Clear();
            _deadFood.Clear();
            CurrentAction = PlayerActionEnum.Right; // TODO: fire Event, maybe?
            await Task.CompletedTask;
        }

        // Gets executed on Tick until current game is over
        public async Task Tick(CancellationToken cancellationToken = default) 
        {
            if(!cancellationToken.IsCancellationRequested) 
            {
                var head = _snake.Last();
                var tail = _snake.First();
                (int X, int Y) newHead = (head.X, head.Y);

                // Game Over Conditions
                if (head.X == StartX                    // Hit left
                        || head.X == StartX + Width     // Hit right
                        || head.Y == StartY             // Hit top
                        || head.Y == StartY + Height    // Hit bottom
                        || _snake.Count(snakePiece => snakePiece.Equals(newHead)) >= 2   // In itself
                        || _deadFood.Contains(newHead)) // Hit dead food
                        
                {
                    OnGameOver?.Invoke(this, new EventArgs());
                    return;
                }

                // Put food
                await AddFood(cancellationToken);

                // Move Snake
                if (CurrentAction == PlayerActionEnum.Right)
                    newHead = (head.X + 1, head.Y);
                else if (CurrentAction == PlayerActionEnum.Down)
                    newHead = (head.X, head.Y + 1);
                else if (CurrentAction == PlayerActionEnum.Left)
                    newHead = (head.X - 1, head.Y);
                else if (CurrentAction == PlayerActionEnum.Up)
                    newHead = (head.X, head.Y - 1);
                
                _snake.Add(newHead);
                await _inputOutputService.Print(newHead.X, newHead.Y, _snakeHeadPiece, cancellationToken);
                await _inputOutputService.Print(head.X, head.Y, _snakePiece, cancellationToken);
                
                if (_food.Contains(newHead)) {
                    // Hit the Food
                    OnFoodHit?.Invoke(this, new EventArgs());
                    _food.Remove(newHead);

                    // Refresh position of all 'death' pieces every food hit
                    Task.WaitAll(
                        _deadFood.Select(deadFoodPiece => _inputOutputService.Print(deadFoodPiece.X, deadFoodPiece.Y, " ", cancellationToken)).ToArray()
                    );
                    _deadFood.Clear();
                } else {
                    await _inputOutputService.Print(tail.X, tail.Y, " ", cancellationToken);
                    _snake.Remove(tail);
                }
            }

            await Task.CompletedTask;
        }

        private async Task AddFood(CancellationToken cancellationToken = default)
        {
            // Add Normal Food
            while(_food.Count < Constants.MAX_NORMAL_FOOD_COUNT)
            {
                (int X, int Y) newFood = (_randomizer.Next(StartX + Constants.BORDER_WIDTH, StartX + Width), 
                    _randomizer.Next(StartY + Constants.BORDER_WIDTH, StartY + Height - Constants.BORDER_WIDTH));
                
                if (!_snake.Contains(newFood) && !_food.Contains(newFood))
                {
                    await _inputOutputService.Print(newFood.X, newFood.Y, (255, 255, 0), _foodPiece, cancellationToken);
                    _food.Add(newFood);
                }
            }

            // Add Death Block(s)  
            while(_deadFood.Count < Constants.MAX_DEAD_FOOD_COUNT)
            {
                var snakeHead = _snake.Last();  
                (int X, int Y) deadFood = (
                    _randomizer.Next(StartX + Constants.BORDER_WIDTH, StartX + Width), 
                    _randomizer.Next(StartY + Constants.BORDER_WIDTH, StartY + Height - Constants.BORDER_WIDTH));

                // Not closer than 5 symbols to head towards CurrentAction 
                if (CurrentAction == PlayerActionEnum.Left && deadFood.X >= snakeHead.X - 10 && deadFood.X <= snakeHead.X
                    || CurrentAction == PlayerActionEnum.Right && deadFood.X <= snakeHead.X + 10 && deadFood.X >= snakeHead.X
                    || CurrentAction == PlayerActionEnum.Up && deadFood.Y >= snakeHead.Y - 10 && deadFood.Y <= snakeHead.Y
                    || CurrentAction == PlayerActionEnum.Down && deadFood.Y >= snakeHead.Y + 10  && deadFood.Y >= snakeHead.Y
                    || _snake.Contains(deadFood)
                    || _food.Contains(deadFood)
                    || _deadFood.Contains(deadFood))
                {
                    continue;
                }
                
                await _inputOutputService.Print(deadFood.X, deadFood.Y, (255, 0, 0), _deadFoodPiece, cancellationToken);
                _deadFood.Add(deadFood);
            }
        }
    }
}