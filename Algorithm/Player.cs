using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Algorithm
{
    class Pos
    {
        public Pos(int y, int x) { Y = y; X = x; }
        public int Y;
        public int X;
    }

    class Player
    {
        public int PosX { get; private set; }
        public int PosY { get; private set; }
        Random _random = new Random();

        Board _board;

        enum Dir
        {   // 반시계 방향
            Up, // 3 -> 3
            Left, // 4 -> 0
            Down, // 5 -> 1
            Right // 6 -> 2
        }

        int _dir = (int)Dir.Up;
        List<Pos> _points = new List<Pos>();

        public void Initialize(int posY, int posX, Board board)
        {
            PosY = posY;
            PosX = posX;
            _board = board;

            // RightHand();
            // BFS();
            AStar();
        }
        
        struct PQNode : IComparable<PQNode>
        {
            public int F;
            public int G;
            public int Y;
            public int X;

            public int CompareTo(PQNode other)
            {
                if (F == other.F)
                    return 0;
                // F값이 작을수록 좋으니.. 비교하는 F값보다 작다면 1을 반환시킴
                return F < other.F ? 1 : -1;
            }
        }


        private void AStar()
        {
            // up, left, down, right + UL, DL, DR, UR 4방향 추가
            int[] deltaY = new int[] { -1, 0, 1, 0, -1, 1, 1, -1 };
            int[] deltaX = new int[] { 0, -1, 0, 1, -1, -1, 1, 1 };
            // 상하좌우 1 -> 10 , 대각선 1.4 -> 14
            int[] cost = new int[] { 10, 10, 10, 10, 14, 14, 14, 14 };

            // 점수 매기기
            // F = G + H
            // F = 최종 점수 (작을 수록 좋음, 경로에 따라 달라짐)
            // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을수록 좋다, 경로에 따라 달라짐)
            // H = 목적지에서 얼마나 가까운지 (작을수록 좋다, 고정값)

            // (y, x) 이미 방문했는지 여부를 기록 ( 방문 = closed 상태, 이전 visited, found 상태와 동일)
            bool[,] closed = new bool[_board.Size, _board.Size]; // ClosedList

            // (y, x) 가는 길을 한번이라도 발견했는지?
            // 발견 X => Int.MaxValue
            // 발견 O => F = G + H
            int[,] open = new int[_board.Size, _board.Size]; // OpenList
            for (int y = 0; y < _board.Size; y++)
                for (int x = 0; x < _board.Size; x++)
                    open[y, x] = Int32.MaxValue;

            Pos[,] parent = new Pos[_board.Size, _board.Size];

            // 가장 좋은 값 하나만 뽑는대 최적화된 자료구조임
            // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

            // 시작점 발견 (에약 진행)
            // 시작점의 경우 G값은 0 (자신이 시작점임)
            // 시작점의 경우 F = H
            open[PosY, PosX] = 10 * Math.Abs(_board.DestY - PosY) + Math.Abs(_board.DestX - PosX);
            pq.Push(new PQNode() { 
                F = 10 * (Math.Abs(_board.DestY - PosY) + Math.Abs(_board.DestX - PosX)), 
                G = 0, Y = PosY, X = PosX });
            parent[PosY, PosX] = new Pos(PosY, PosX);

            while (pq.Count > 0)
            {
                // 제일 좋은 후보를 찾는다.
                // 모든 지점을 뒤져보면서 F값들을 비교하는것은 너무 오래걸림
                PQNode node = pq.Pop();

                // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해 이미 방문(closed)된 경우 스킵
                if (closed[node.Y, node.X])
                    continue;

                // 아니라면 방문한다.
                closed[node.Y, node.X] = true;
                // 목적지에 도착했다면 바로 종료
                if (node.Y == _board.DestY && node.X == _board.DestX)
                    break;

                // 상하좌우 이동할 수 있는 좌표인지 확인해서 예약(open) 한다.
                for(int i = 0; i < deltaY.Length; i++)
                {
                    int nextY = node.Y + deltaY[i];
                    int nextX = node.X + deltaX[i];

                    // 각 좌표에 대해 체크
                    // 배열 범위를 벗어나면 스킵
                    if (nextX < 0 || nextX >= _board.Size || nextY < 0 || nextY >= _board.Size)
                        continue;
                    // 벽으로 막힌 경우 스킵
                    if (_board.Tile[nextY, nextX] == Board.TileType.Wall)
                        continue;
                    // 이미 방문했다면 스킵
                    if (closed[nextY, nextX])
                        continue;

                    // 비용 계산
                    // 이전까지 이동했던 G값 + 특정값
                    int g = node.G + cost[i];
                    int h = 10 * (Math.Abs(_board.DestY - nextY) + Math.Abs(_board.DestX - nextX));
                    // 다른 경로에서 더 빠른 길을 이미 찾았으면 스킵.
                    if (open[nextY, nextX] < g + h)
                        continue;

                    // 여기까지 왔으면 경로까지 가장 빠른길이라는 소리
                    open[nextY, nextX] = g + h;
                    // 예약 진행
                    pq.Push(new PQNode() { F = g + h, G = g, Y = nextY, X = nextX });
                    // node YX덕에 next YX를 찾았음
                    parent[nextY, nextX] = new Pos(node.Y, node.X);
                }
            }

            CalcPathFromParent(parent);
        }

        // BFS로 길찾기는 모든 간선의 가중치가 동일해야만 가능
        void BFS()
        {
            // up, left, down, right
            int[] deltaY = new int[] { -1, 0, 1, 0 };
            int[] deltaX = new int[] { 0, -1, 0, 1 };

            bool[,] found = new bool[_board.Size, _board.Size];
            Pos[,] parent = new Pos[_board.Size, _board.Size];

            Queue<Pos> q = new Queue<Pos>();
            q.Enqueue(new Pos(PosY, PosX));
            found[PosY, PosX] = true;
            // 시작점은 자기 자신이 부모
            parent[PosY, PosX] = new Pos(PosY, PosX);

            while(q.Count > 0)
            {
                Pos pos = q.Dequeue();
                int nowY = pos.Y;
                int nowX = pos.X;

                // 자기 기준으로 상하좌우 확인
                for(int i = 0; i < 4; i++)
                {
                    int nextY = nowY + deltaY[i];
                    int nextX = nowX + deltaX[i];

                    // 배열 범위를 벗어나지 않게 조심
                    if (nextX < 0 || nextX >= _board.Size || nextY < 0 || nextY >= _board.Size)
                        continue;
                    
                    if (_board.Tile[nextY, nextX] == Board.TileType.Wall)
                        continue;
                    if (found[nextY, nextX])
                        continue;

                    q.Enqueue(new Pos(nextY, nextX));
                    found[nextY, nextX] = true;
                    parent[nextY, nextX] = new Pos(nowY, nowX); // 새로 방문하게될 지점의 부모
                }
            }
            // 여기까지 하면 경로상 모든 점의 부모 위치 저장됨.
            CalcPathFromParent(parent);
        }

        void CalcPathFromParent(Pos[,] parent)
        {
            // 목적지로부터 시작점까지 거슬러올라감
            int y = _board.DestY;
            int x = _board.DestX;
            while (parent[y, x].Y != y || parent[y, x].X != x)
            {
                _points.Add(new Pos(y, x));
                // 부모노드 좌표로 이동
                Pos pos = parent[y, x];
                y = pos.Y;
                x = pos.X;
            }
            // 여기서의 y, x좌표는 시작점
            _points.Add(new Pos(y, x)); // 끝점에 도달하는 순간 _points에 add를 안하고 빠져나오기 때문에
            // 역순서로 들어왔으니 뒤집자
            _points.Reverse();
        }

        private void RightHand()
        {
            // 현재 바라보고 있는 방향을 기준으로, 좌표 변화를 나타낸다. 
            // 위, 왼, 아래, 오 순서
            int[] frontY = new int[] { -1, 0, 1, 0 };
            int[] frontX = new int[] { 0, -1, 0, 1 };
            int[] rightY = new int[] { 0, -1, 0, 1 };
            int[] rightX = new int[] { 1, 0, -1, 0 };

            _points.Add(new Pos(PosY, PosX));
            // 목적지 도착하기 전에는 계속 실행
            while (PosY != _board.DestY || PosX != _board.DestX)
            {
                // 내가 바라보는 방향에 따라 오른쪽의 기준이 달라진다.
                // 1. 현재 바라보는 방향을 기준으로 오른쪽으로 갈 수 있는지 확인.
                if (_board.Tile[PosY + rightY[_dir], PosX + rightX[_dir]] == Board.TileType.Empty)
                {
                    // 오른쪽 방향으로 90도 회전
                    _dir = (_dir - 1 + 4) % 4; // %n을 하면 나오는 값이 0~n범위에서 유지

                    // 앞으로 한 보 전진
                    PosY = PosY + frontY[_dir];
                    PosX = PosX + frontX[_dir];
                    _points.Add(new Pos(PosY, PosX));

                }
                // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인.
                else if (_board.Tile[PosY + frontY[_dir], PosX + frontX[_dir]] == Board.TileType.Empty)
                {
                    // 앞으로 한 보 전진
                    PosY = PosY + frontY[_dir];
                    PosX = PosX + frontX[_dir];
                    _points.Add(new Pos(PosY, PosX));
                }
                else
                {
                    // 왼쪽 방향으로 90도 회전
                    _dir = (_dir + 1 + 4) % 4;
                }
            }
        }

        const int MOVE_TICK = 30; // 100ms = 0.1s
        int _sumTick = 0;
        int _lastIndex = 0;
        public void Update(int deltaTick)
        {
            if (_lastIndex >= _points.Count)
            {
                // 도착했으면 맵을 다시 생성
                _lastIndex = 0;
                _points.Clear();
                _board.Initialize(_board.Size, this);
                Initialize(1, 1, _board);
            }

            // 여긴 1/30초로 들어오면 너무 빠름
            // deltaTick 줄태니 알아서 해라
            _sumTick += deltaTick;
            if (_sumTick >= MOVE_TICK)
            {
                _sumTick = 0;

                PosY = _points[_lastIndex].Y;
                PosX = _points[_lastIndex].X;
                _lastIndex++;
            }
        }
    }
}
