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
            public int F; // 최종점수
            public int G; // 시작점부터 목적지까지 이동하는데 드는 비용
            // H는 F와 G가 있으면 구할수있으니 뺌
            public int Y;
            public int X;

            public int CompareTo(PQNode other)
            {
                if (F == other.F)
                    return 0;
                // F값이 작을수록 우선순위를 높게 준다.
                // other.F보다 this.F가 더 작으면 1을 반환.
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
            // H = 목적지에서 얼마나 가까운지 (작을수록 좋다, 고정값, 가산점) -> 특정 좌표로부터 목적지까지의 거리는 고정임

            // (y, x) 이미 방문했는지 여부를 기록 ( 방문 = closed 상태, 이전 visited, found 상태와 동일)
            bool[,] closed = new bool[_board.Size, _board.Size]; // ClosedList

            // open 배열에 들어간 최종점수의 의미는?
            //  1. open 배열에는 어떤 경로에서 배열의 인덱스를 y,x 좌표라 하고
            //  2. 이 좌표에서 목적지까지 가는 최종점수(F)를 저장해 놓은 배열이다.
            //  3. 내가 진행중인 경로 기준으로 계산한 값보다 더 적은 평가점수가 이미 들어가 있다면
            //  4. 이전 경로 기준으로 해당 좌표를 접근하는게 더 빠르기 때문에 현재 진행중인 경로에서는 방문 후보 리스트(pq)에 넣지 않는다.
            int[,] open = new int[_board.Size, _board.Size]; // OpenList
            for (int y = 0; y < _board.Size; y++)
                for (int x = 0; x < _board.Size; x++)
                    open[y, x] = Int32.MaxValue;
            // (y, x) 가는 길을 한번이라도 발견했는가?
            //  발견 X => Int.MaxValue
            //  발견 O => F = G + H

            Pos[,] parent = new Pos[_board.Size, _board.Size];

            // 가장 좋은 값 하나만 뽑는대 최적화된 자료구조임
            // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

            // 시작점 발견 (에약 진행)
            //  시작점의 G값은 0 (자신이 시작점임) 이므로, F = H 이다.
            //  H값은 맨해튼 거리 공식을 이용한다.
            //  open 배열은 해당 인덱스의 좌표의 F값을 들고있다.
            //  F값을 구할때 왜 10을 곱하냐면 cost(한 칸 이동의 비용)이 1에서 10으로 바뀌었기 때문에
            //  H값(시작점부터 목적지 까지 가는 거리)에 새롭게 바뀐 이동비용 10을 곱해야 G값과 형평성이 맞다.
            //  G값의 경우 구하는 과정에서 cost를 적용하기 때문에 따로 10을 곱할 필요가 없다.
            open[PosY, PosX] = 10 * (Math.Abs(_board.DestY - PosY) + Math.Abs(_board.DestX - PosX));
            // 시작점의 평가점수를 우선순위 큐에 삽입
            pq.Push(new PQNode() { F = 10 * (Math.Abs(_board.DestY - PosY) + Math.Abs(_board.DestX - PosX)), G = 0, Y = PosY, X = PosX });
            parent[PosY, PosX] = new Pos(PosY, PosX); // 시작점은 이전경로가 없으니 자기 자신을 넣는다.

            while (pq.Count > 0)
            {
                // 제일 좋은 후보를 찾는다.
                // 루프돌면서 주변 좌표의 모든 F점수를 비교하는것은 너무 오래걸림
                PQNode node = pq.Pop(); // 우선순위 큐는 위와같은 일에 최적화 되어있다

                // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해 이미 방문(closed)된 경우 스킵
                // 이전 경로탐색때 방문했던 좌표니까, 다시 갈 필요가 없다.
                if (closed[node.Y, node.X])
                    continue;

                // 가본적이 없고 내가 갈 수 있는 좌표중에 가장 가점이 높다 -> 방문
                closed[node.Y, node.X] = true; // 방문체크, 다시는 안 간다.
                
                // 방문할 좌표가 목적지 좌표면 탐색 종료.
                if (node.Y == _board.DestY && node.X == _board.DestX)
                    break;

                // node.Y, node.X 좌표에 방문완료
                // 이거 기준으로 (up, left, down, right + UL, DL, DR, UR 4방향) 총 8방향 확인
                // 이동 가능한지 체크하고 우선순위 큐에 추가
                for (int i = 0; i < deltaY.Length; i++)
                {
                    int nextY = node.Y + deltaY[i];
                    int nextX = node.X + deltaX[i];

                    // 못가는 경우 체크
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
                    // 시작점부터 node 위치까지 가는 비용 G + 이후 진행방향별 보정치
                    int g = node.G + cost[i];
                    int h = 10 * (Math.Abs(_board.DestY - nextY) + Math.Abs(_board.DestX - nextX));
                    // 다른 경로에서 이미 더 빠른길 찾아놨으면 스킵
                    if (open[nextY, nextX] < g + h)
                        continue;

                    // 여기까지 왔으면 nextY, nextX 좌표를 거치면서 목적지로 가는 경로중에 가장 최적의 경로임
                    open[nextY, nextX] = g + h; // 평가값 갱신
                    // 경로 후보에 넣는다.
                    pq.Push(new PQNode() { F = g + h, G = g, Y = nextY, X = nextX });
                    // 부모(직전경로)를 등록해서 쭉 따라가면 최적경로 이동 되는거임
                    // nextY, nextX 좌표로 가장 빠르게 이동하기 위한 직전 좌표는 node.Y, node.X
                    parent[nextY, nextX] = new Pos(node.Y, node.X);
                }
            }

            // 부모 좌표를 타고 올라가면서 경로를 만든다.
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
