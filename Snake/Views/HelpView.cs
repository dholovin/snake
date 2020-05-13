using System.Threading;
using System.Threading.Tasks;
using Snake.Common;
using Snake.ServiceContracts.Interfaces;

namespace Snake.Views
{
    public class HelpView : BasePanel
    {
        private IInputOutputService _inputOutputService;

        public HelpView(int startX, int startY, int width, int height, IInputOutputService io) 
            : base(startX, startY, width, height)
        {
            _inputOutputService = io;
        }
    } 
}