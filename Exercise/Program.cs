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
