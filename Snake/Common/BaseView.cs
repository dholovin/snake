using System.Threading;
using System.Threading.Tasks;

namespace Snake.Common
{
    public abstract class BaseView
    {
        public int StartX { get; private set; }
        public int StartY { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        // To be used with DI
        public BaseView() 
        {
        }

        // Can't be used with DI
        public  BaseView(int startX, int startY, int width, int height) 
        {
            StartX = startX;
            StartY = startY;
            Width = width;
            Height = height;
        }

        public virtual async Task SetDimensions(int startX, int startY, int width, int height,
            CancellationToken cancellationToken = default)
        {
            StartX = startX;
            StartY = startY;
            Width = width;
            Height = height;

            await Task.CompletedTask;
        }
    } 
}