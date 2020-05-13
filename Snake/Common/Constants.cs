namespace Snake.Common
{
    public static class Constants
    {
        public const int LEVEL_SPEED_MULTIPLIER = 50;

        // Layout
        public const int PADDING = 1;
        public const int MAIN_SCREEN_SIZE_MULTIPLIER = 1;
        public const int HELP_PANE_WIDTH = 20;
        public const int SUMMARY_PANE_HEIGHT = 5;
        public const int MAIN_SCREEN_MIN_WIDTH = 40;
        public const int MAIN_SCREEN_MIN_HEIGHT = 20;


        // MESSAGES
        public const string NOT_INITIALIZED_ERROR = "Game is not initialized.";
        public const string SCREEN_RESOLUTION_ERROR = "The game has been designed for screen {0} x {1} symbols. Please adjust terminal window size.";
        public const string SHOULD_PLAY_AGAIN = "Press 'Y' to play again. Press 'N' or ESC to exit: ";
    }
}