using System.Threading;
using System.Threading.Tasks;
using Snake.Common.Enums;

namespace Snake.ServiceContracts.Interfaces
{
    public interface IInputOutputService
    {
        public Task<(int Width, int Height)> GetViewportDimensions(CancellationToken cancellationToken);
        public Task Clear(CancellationToken cancellationToken);
        public Task Print(object message, CancellationToken cancellationToken);   
        public Task Print(int x, int y, object message, CancellationToken cancellationToken);
        public Task Print(int x, int y, (byte R, byte G, byte B) color, object message, CancellationToken cancellationToken);
        public Task PrintAtNextLine(object message, CancellationToken cancellationToken);
        public Task PrintAtNextLine(int x, int y, (byte R, byte G, byte B) color, object message, CancellationToken cancellationToken);
        
        public Task<byte?> GetKey(CancellationToken cancellationToken);
        public Task<string> GetString(CancellationToken cancellationToken);
        public Task Terminate(CancellationToken cancellationToken);    


        public Task<PlayerActionEnum> GetPlayerAction(CancellationToken cancellationToken); // Lives here, can it be different for different devices?
    } 
}