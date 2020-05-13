using System.Threading;
using System.Threading.Tasks;

namespace Snake.ServiceContracts.Interfaces
{
    public interface IInputOutputService
    {
        public Task<(int Width, int Height)> GetViewportDimensions(CancellationToken cancellationToken);
        public Task Clear(CancellationToken cancellationToken);
        public Task Print(string message, CancellationToken cancellationToken);   
        public Task Print(int x, int y, string message, CancellationToken cancellationToken);
        //public Task Print(int x, int y, int width, string message, CancellationToken cancellationToken);
        public Task PrintAtNextLine(string message, CancellationToken cancellationToken);
        public Task PrintAtNextLine(int x, int y, string message, CancellationToken cancellationToken);
        
        public Task<byte?> GetKey(CancellationToken cancellationToken);
        public Task<string> GetString(CancellationToken cancellationToken);
        public Task Terminate(CancellationToken cancellationToken);       
    } 
}