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
        private readonly string _deadPiece;

        private readonly Random _randomizer;

        // TODO: think if redesigning to arrays make core shorter and logic simpler/more complicated?
        // private (int X, int Y)[] _snake; // ushort 0 to 65,535, 16 bit
        private List<(int X, int Y)> _snake = new List<(int X, int Y)>(); // TODO: Ensure thread-safety
        private List<(int X, int Y)> _food = new List<(int X, int Y)>(); // TODO: Ensure thread-safety

        public event EventHandler OnGameOver;
        public event EventHandler OnFoodHit;
        
        public PlayerActionEnum CurrentAction { get; set; }

        public MainView(IInputOutputService inputOutputService, IFigureService figureService)
        {
            _inputOutputService = inputOutputService;
            _figureService = figureService;
            
            _randomizer = new Random();
            _snakePiece = _figureService.GetSnakeFigure(CancellationToken.None).Result;
            _snakeHeadPiece = _figureService.GetSnakeHeadFigure(CancellationToken.None).Result;
            _foodPiece = _figureService.GetFoodFigure(CancellationToken.None).Result;
            _deadPiece = _figureService.GetDeadFoodFigure(CancellationToken.None).Result;
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
            CurrentAction = PlayerActionEnum.Right; // TODO: fire Event, maybe?
            await Task.CompletedTask;
        }

        // Gets executed on Tick until current game is over
        public async Task Tick(CancellationToken cancellationToken = default) 
        {
            // TODO: do we need this check?
            if(!cancellationToken.IsCancellationRequested) {
                // await _inputOutputService.Print(3, 3,  _randomizer.Next(100), cancellationToken); // DEBUG
                
                var borderWidth = 1;
                var head = _snake.Last();
                var tail = _snake.First();
                (int X, int Y) newHead = (head.X, head.Y);
                (int X, int Y) newFood;

                // Game Over Conditions
                if (head.X == StartX                                            // Hit left
                        || head.X == StartX + Width                             // Hit right
                        || head.Y == StartY /*+ borderWidth*/                   // Hit top
                        || head.Y == StartY + Height /*- borderWidth*/          // Hit bottom
                        //|| _snake.Count(snakePiece => snakePiece.X == newHead.X && snakePiece.Y == newHead.Y) == 2)  // In itself
                        || _snake.Count(snakePiece => snakePiece.Equals(newHead)) >= 2)  // In itself
                {
                    OnGameOver?.Invoke(this, new EventArgs());
                    return; // Important to break here, not just fire an event
                }

                // Put food
                while(_food.Count < 1)
                {
                    newFood = (_randomizer.Next(StartX + borderWidth, StartX + Width), 
                        _randomizer.Next(StartY + borderWidth, StartY + Height - borderWidth));

                    if (!_snake.Contains(newFood))
                    {
                        _food.Add(newFood);
                        await _inputOutputService.Print(newFood.X, newFood.Y, _foodPiece, cancellationToken);
                    }
                }

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
                    OnFoodHit?.Invoke(this, new EventArgs());
                    _food.Remove(newHead);
                } else {
                    await _inputOutputService.Print(tail.X, tail.Y, " ", cancellationToken);
                    _snake.Remove(tail);
                }
            }

            await Task.CompletedTask;
        }
    } 
}