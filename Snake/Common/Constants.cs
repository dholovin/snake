namespace Snake.Common
{
    public static class Constants
    {
        public const int SCORE_INCREMENT = 10; 
        public const int SCORE_PER_LEVEL_INCREMENT = 10 * SCORE_INCREMENT;
        public const int LEVEL_SPEED_MULTIPLIER = 50;     // normal for level 1
        // public const int LEVEL_SPEED_MULTIPLIER = 10;    // fast
        //public const int LEVEL_SPEED_MULTIPLIER = 100;   // slow

        // Layout
        public const int PADDING = 1;
        public const int MAIN_SCREEN_SIZE_MULTIPLIER = 1;
        public const int HELP_PANE_WIDTH = 20;
        public const int SUMMARY_PANE_HEIGHT = 5;
        public const int MAIN_SCREEN_MIN_WIDTH = 40;
        public const int MAIN_SCREEN_MIN_HEIGHT = 20;


        // MESSAGES
        public const string INPUT_LEVEL = "Your Level (0-9): ";
        public const string NOT_INITIALIZED_ERROR = "Game is not initialized.";
        public const string SCREEN_RESOLUTION_ERROR = "The game has been designed for screen {0} x {1} symbols. Please adjust terminal window size.";
        public const string SHOULD_PLAY_AGAIN = "Press 'Y' to play again. Press 'N' or ESC to exit: ";
        public const string GameCopyright = "\r\nSNAKE (c) 2020 by Denys Holovin.\r\n\r\n" +
                                            "Homepage url: https://github.com/dholovin/snake/ \r\n" +
                                            "Inspired by Andriy S\'omak, https://github.com/semack/terminal-tetris/\r\n\r\n";
    }
}