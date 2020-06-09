using System;
using System.Collections.Generic;

namespace Exercise
{
    class PriorityQueue
    {
        List<int> _heap = new List<int>();

        public void Push(int data)
        {
            // 힙의 맨 끝에 새로운 데이터를 삽입
            _heap.Add(data);

            // 힙의 맨 위로 올려보자 (가능하다면)
            int now = _heap.Count - 1;
            while(now > 0)
            {
                int next = (now - 1) / 2; // 부모 인덱스 구하기
                if (_heap[now] < _heap[next])
                    break; // 부모보다 작아서 못올라간다.

                // 크다면 교체
                int temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;

                // 검사 위치를 이동
                now = next;
            }
        }

        // 가장 큰 데이터를 넘긴다.
        public int Pop()
        {
            // 반환할 데이터를 따로 저장
            int ret = _heap[0];

            // 마지막 데이터를 루트로 이동.
            int lastIndex = _heap.Count - 1;
            _heap[0] = _heap[lastIndex];
            _heap.RemoveAt(lastIndex);
            lastIndex--;

            // 역으로 내려가기
            int now = 0;
            while (true)
            {
                // left가 범위 밖(없는) 노드를 체크할수도 있음
                int left = 2 * now + 1;
                int right = 2 * now + 2;

                int next = now;
                // 왼쪽값이 현재값보다 크면, 왼쪽으로 이동
                if (left <= lastIndex && _heap[next] < _heap[left])
                    next = left;
                // 오른쪽값이 현재값(현재기준 왼쪽값 포함)보다 크면, 오른쪽으로 간다.
                if (right <= lastIndex && _heap[next] < _heap[right])
                    next = right;

                // 왼쪽 오른쪽 모두 현재값보다 작으면 끝
                if (next == now)
                    break;

                // 두 값을 교체하자
                int temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;

                // 검사 위치를 이동한다.
                now = next;
            }


            return ret;
        }

        public int Count()
        {
            return _heap.Count;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            PriorityQueue q = new PriorityQueue();
            q.Push(20);
            q.Push(10);
            q.Push(30);
            q.Push(90);
            q.Push(40);

            while(q.Count() > 0)
            {
                Console.WriteLine(q.Pop());
            }
        }
    }
}
