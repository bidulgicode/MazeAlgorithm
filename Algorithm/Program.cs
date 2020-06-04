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
            Player player = new Player();
            // 플레이어를 움직이는것은 플레이어 클래스 내부에서 해야한다
            // 움직이려면 맵 정보를 알아야함.
            board.Initialize(MAP_SIZE, player);
            player.Initialize(1, 1, board);

            Console.CursorVisible = false;

            int lastTick = 0;
            while (true)
            {
                int currentTick = System.Environment.TickCount;
                if (currentTick - lastTick < WAIT_TICK)
                    continue;

                int deltaTick = currentTick - lastTick;
                lastTick = currentTick;

                // 입력 

                // 로직
                player.Update(deltaTick);

                // 랜더
                Console.SetCursorPosition(0, 0);
                board.Render();
            }
        }
    }
}
