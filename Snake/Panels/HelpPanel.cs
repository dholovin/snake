using System.Threading;
using System.Threading.Tasks;
using Snake.Common;
using Snake.ServiceContracts.Interfaces;

namespace Snake.Panels
{
    public class HelpPanel : BasePanel
    {
        private IInputOutputService _inputOutputService;

        public HelpPanel(int startX, int startY, int width, int height, IInputOutputService io) 
            : base(startX, startY, width, height)
        {
            _inputOutputService = io;
        }
    } 
}