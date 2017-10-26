using System;

namespace Play_As_You_Go
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Play_As_You_Go game = new Play_As_You_Go())
            {
                game.Run();
            }
        }
    }
#endif
}

