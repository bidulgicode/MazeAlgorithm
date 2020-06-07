using System;
using System.Collections.Generic;

namespace Exercise
{
    class Program
    {
        class Graph
        {
            // 3과 4 노드가 끊겨있다.
            int[,] adj = new int[6, 6]
            {
                { 0, 1, 0, 1, 0, 0 },
                { 1, 0, 1, 1, 0, 0 },
                { 0, 1, 0, 0, 0, 0 },
                { 1, 1, 0, 0, 1, 0 },
                { 0, 0, 0, 1, 0, 1 },
                { 0, 0, 0, 0, 1, 0 },
            };

            List<int>[] adj2 = new List<int>[]
            {
                new List<int>() { 1, 3 },
                new List<int>() { 0, 2, 3 },
                new List<int>() { 1 },
                new List<int>() { 0, 1, 4 },
                new List<int>() { 3, 5 },
                new List<int>() { 4 },
            };

            bool[] visited = new bool[6];
            // 1. now 부터 방문
            // 2. now와 연결된 정점들을 하나씩 확인해사, 아직 미방문 상태라면 방문.
            public void DFS(int now)
            {
                Console.WriteLine(now);
                visited[now] = true; // 1. now부터 방문.

                // adj.GetLength(0); == 6
                for(int next = 0; next < 6; next++)
                {
                    if (adj[now, next] == 0)
                        continue;   // 연결되지 않았다면 스킵.
                    if (visited[next]) // 이미 방문했다면 스킵.
                        continue;
                    DFS(next); // 나와 연결된 정점에도 같은 일 수행
                }
            }

            public void DFS2(int now)
            {
                Console.WriteLine(now);
                visited[now] = true;

                foreach (var next in adj2[now]) // 연결된 것들만 넣어놨기 때문에 따로 체크할 필요가 없음.
                {
                    if (visited[next]) // 이미 방문했다면 스킵.
                        continue;
                    DFS2(next);
                }
            }

            // 정점 연결이 끊겨있는 경우 그 정점은 DFS 재귀를 통해 호출되지 않기 때문에
            // 찾아서 호출해줘야함.
            public void SearchAll() 
            {
                visited = new bool[6];
                for (int now = 0; now < 6; now++)
                    // DFS를 실행하는 순간 연결되어있던 
                    // 모든 정점은 ture가 되기 때문에 의외로 체크를 적게 한다.
                    if (visited[now] == false) 
                        DFS(now);
            }
        }
        // 스택 : LIFO
        // 큐 : FIFO
        static void Main(string[] args)
        {
            // DFS 깊이우선
            Graph graph = new Graph();
            //graph.DFS(3); // 시작점에 따라 탐색순서가 달라짐.
            graph.SearchAll();
            
            // BFS 너비우선

        }
    }
}
