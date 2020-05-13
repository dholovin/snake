namespace Snake.Common
{
    public abstract class BasePanel
    {
        public int StartX { get; private set; }
        public int StartY { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public BasePanel(int startX, int startY, int width, int height) 
        {
            StartX = startX;
            StartY = startY;
            Width = width;
            Height = height;
        }
    } 
}