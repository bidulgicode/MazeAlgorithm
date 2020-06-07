using System;
using System.Collections.Generic;

namespace Exercise
{
    class Program
    {
        class Graph
        {
            // 3과 4 노드가 끊겨있다.
            // 0과 1이 아닌 가중치 값으로 수정 -> 다익스트라
            int[,] adj = new int[6, 6]
            {
                { -1, 15, -1, 35, -1, -1 },
                { 15, -1, 5, 10, -1, -1 },
                { -1, 5, -1, -1, -1, -1 },
                { 35, 10, -1, -1, 5, -1 },
                { -1, -1, -1, 5, -1, 15},
                { -1, -1, -1, -1, 5, -1 },
            };

            public void Dijkstra(int start)
            {
                // 노드를 찾아낸것보다 실제로 방문했는가가 중요함
                bool[] visited = new bool[6];
                // 최단거리 저장
                int[] distance = new int[6];
                int[] parent = new int[6]; // 경로 추적용

                Array.Fill(distance, Int32.MaxValue);

                distance[start] = 0;
                parent[start] = start;

                // 이 부분이 이중포문마냥 돌아서 겁나 느림
                while (true)
                {
                    // 제일 좋은 후보를 찾는다 (가장 가까이에 있는 놈)

                    // 가장 유력한 후보의 거리와 번호를 저장
                    int closest = Int32.MaxValue;
                    int now = -1;

                    for(int i = 0; i < 6; i++)
                    {
                        // 이미 방문한 정점은 스킵한다.
                        if (visited[i])
                            continue;
                        // 아직 발견된(예약) 적이 없거나, 기존 후보보다 멀다면 스킵
                        if (distance[i] == Int32.MaxValue || distance[i] >= closest)
                            continue;
                        // 여기까지 오면 가장 좋은 후보라는 의미
                        closest = distance[i];
                        now = i;
                    }
                    // for문 이후에 closest와 now는 가장 좋은 값이 되어있다 or 변화없음.

                    // 다음 후보가 하나도 없다 -> 종료
                    // 모든 점을 찾아봤거나, 단절되어 있다.
                    if (now == -1)
                        break;

                    // 제일 좋은 후보를 찾았음. -> 방문
                    visited[now] = true;
                    
                    // 방문한 정점과 인접한 정점들을 조사해서
                    // 상황에 따라 발견한 최단거리를 갱신
                    for(int next = 0; next < 6; next++)
                    {
                        // 연결되지 않은 정점은 스킵
                        if (adj[now, next] == -1)
                            continue;
                        // 이미 방문한 정점은 스킵
                        if (visited[next])
                            continue;

                        // 여기까지 오면 연결되어있고 방문한적이 없는 정점임
                        // 새로 조사된 정점의 최단거리를 계산한다.
                        int nextDist = distance[now] + adj[now, next];
                        // 만약 기존에 발견된 최단거리가 새로 조사된 최단거리보다 크면
                        // 정보를 갱신
                        if(nextDist < distance[next])
                        {
                            // 초기값일 경우에는 어마어마하게 크니 무조건 갱신됨.
                            distance[next] = nextDist;
                            // next라는 정점은 now를 통해 발견된 것이다.
                            parent[start] = now;
                        }
                        
                    }
                }
            }

            //선입선출 BFS -> Queue
            public void BFS(int start)
            {
                bool[] found = new bool[6];
                int[] parent = new int[6];
                int[] distance = new int[6];

                Queue<int> q = new Queue<int>();
                q.Enqueue(start);
                found[start] = true;
                parent[start] = start;
                distance[start] = 0;

                while(q.Count > 0)
                {
                    int now = q.Dequeue();
                    Console.WriteLine(now);

                    for(int next = 0; next < 6; next++)
                    {
                        if (adj[now, next] == 0) // 인접하지 않은 노드면 스킵
                            continue;
                        if (found[next]) // 이미 방문한 노드면 스킵
                            continue;
                        q.Enqueue(next);
                        found[next] = true;
                        parent[next] = now;
                        distance[next] = distance[now] + 1;
                    }
                }
            }
        }
        // 스택 : LIFO
        // 큐 : FIFO
        static void Main(string[] args)
        {
            // DFS 깊이우선
            Graph graph = new Graph();
            // graph.DFS(3); // 시작점에 따라 탐색순서가 달라짐.
            // graph.SearchAll();
            
            // BFS 너비우선 -길찾기에 주로 사용됨
            graph.BFS(0);

        }
    }
}
