using System;

namespace Algorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            const int MAP_SIZE = 25;
            const int WAIT_TICK = 1000 / 30;
            
            Board board = new Board();
            board.Initialize(MAP_SIZE);

            Console.CursorVisible = false;

            int lastTick = 0;
            while (true)
            {
                int currentTick = System.Environment.TickCount;
                if (currentTick - lastTick < WAIT_TICK)
                    continue;
                lastTick = currentTick;

                // 입력 
                // 로직
                // 랜더
                Console.SetCursorPosition(0, 0);
                board.Render();

            }
        }
    }
}
